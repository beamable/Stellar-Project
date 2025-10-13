using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Accounts.Models;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.LockManager;
using Beamable.StellarFederation.Features.Stellar;
using Beamable.StellarFederation.Features.Stellar.Models;
using Beamable.StellarFederation.Features.Transactions;
using Beamable.StellarFederation.Features.Transactions.Storage.Models;
using Beamable.StellarFederation.Features.WalletManager.Exceptions;
using Beamable.StellarFederation.Features.WalletManager.Models;

namespace Beamable.StellarFederation.Features.WalletManager;

public class WalletManagerService : IService
{
    private readonly Configuration _configuration;
    private readonly AccountsService _accountsService;
    private readonly LockManagerService _lockManagerService;
    private readonly StellarService _stellarService;

    public WalletManagerService(Configuration configuration, AccountsService accountsService, LockManagerService lockManagerService, StellarService stellarService)
    {
        _configuration = configuration;
        _accountsService = accountsService;
        _lockManagerService = lockManagerService;
        _stellarService = stellarService;
    }

    private const string WorkingAccountNamePrefix = "working-account";
    private const int LockIterationAttempts = 10;
    private const int LockDelayBetweenAttemptsMs = 100;

    public async Task<Account> GetWorkingWallet(TransactionManager transactionManager, StellarLockRequest amountRequest = default)
    {
        try
        {
            amountRequest = amountRequest == default ? StellarLockRequest.Default() : amountRequest;
            var workingAccount = await GetWorkingWalletInternal(amountRequest);
            var workingBalance = await _stellarService.NativeBalance(workingAccount.Address);
            await DistributeCoins(amountRequest, new WalletCoinBalance(workingAccount.Address, workingBalance), transactionManager);
            return workingAccount;
        }
        finally
        {
            await ReleaseWallet(AccountsService.RealmAccountName);
        }
    }

    private async Task ReleaseWallet(string walletName)
    {
        await _lockManagerService.ReleaseLock(walletName);
    }

    private async Task DistributeCoins(StellarLockRequest request, WalletCoinBalance walletCoinBalance, TransactionManager transactionManager)
    {
        if (walletCoinBalance.Balance>= request.Amount) return;

        var realmAccount = await _accountsService.GetOrCreateRealmAccount();
        var realmWalletBalance = await _stellarService.NativeBalance(realmAccount.Address);
        if (realmWalletBalance < request.Amount)
            throw new RealmAccountBalanceException("Realm wallet has insufficient coin balance.");

        await TransferCoins(request, walletCoinBalance, new WalletCoinBalance(realmAccount.Name, realmWalletBalance), transactionManager);
    }

    private async Task TransferCoins(StellarLockRequest request, WalletCoinBalance walletCoinBalance, WalletCoinBalance realmWalletBalance, TransactionManager transactionManager)
    {
        var iterations = 0;
        while (iterations < LockIterationAttempts)
        {
            var realmLocked = await _lockManagerService.AcquireLock(AccountsService.RealmAccountName);
            if (realmLocked)
            {
                var uniqueTransactionId = Guid.NewGuid().ToString();
                var transferAmount = await CalculateTransferAmount(realmWalletBalance);
                if (transferAmount < request.Amount)
                {
                    transferAmount = request.Amount;
                }

                var transactionId = await transactionManager.StartTransaction(0, realmWalletBalance.Wallet, nameof(WalletManagerService), uniqueTransactionId, new { transferAmount });
                transactionManager.SetCurrentTransactionContext(transactionId);
                await transactionManager.RunAsyncBlock(transactionId, uniqueTransactionId, async () =>
                {
                    var result = await _stellarService.TransferNative(walletCoinBalance.Wallet, transferAmount);
                    await transactionManager.AddChainTransaction(new ChainTransaction
                    {
                        Hash = result.Hash,
                        Error = result.Error,
                        Function = $"{nameof(WalletManagerService)}.{nameof(TransferCoins)}",
                        Data = JsonSerializer.Serialize(new { transferAmount }),
                        Status = result.Status.ToString(),
                    });
                    if (result.Status == StellarTransactionStatus.Failed)
                    {
                        var message = $"{nameof(WalletManagerService)}.{nameof(TransferCoins)} failed with status {result.Status}";
                        BeamableLogger.LogError(message);
                        await transactionManager.TransactionError(uniqueTransactionId, new Exception(message));
                    }
                });
                await _lockManagerService.ReleaseLock(AccountsService.RealmAccountName);
                return;
            }
            await Task.Delay(LockDelayBetweenAttemptsMs);
            iterations++;
        }
    }

    private async ValueTask<long> CalculateTransferAmount(WalletCoinBalance realmWalletBalance)
    {
        var equalSplit = WalletManagerExtensions.SafeDivide(realmWalletBalance.Balance, await _configuration.NumberOfWorkingWallets);
        if (equalSplit <= 0)
            return await _configuration.XlmMinimalAmountInStroops;
        var percentageOfEqual = equalSplit * await _configuration.CoinTransferPercentage / 100;
        if (realmWalletBalance.Balance >= percentageOfEqual)
            return percentageOfEqual;
        return await _configuration.XlmMinimalAmountInStroops;
    }

    private async Task<Account> GetWorkingWalletInternal(StellarLockRequest request)
    {
        try
        {
            var lockedWallet = await TryLockWorkingWalletAccount(request);
            return lockedWallet ?? await CreateNewAndLock();
        }
        catch (MaxNumberOfWorkingWallets)
        {
            throw;
        }
        catch (Exception)
        {
            throw new NoWorkingWalletsException();
        }
    }

    private async Task<Account?> TryLockWorkingWalletAccount(StellarLockRequest request)
    {
        var iterations = await _configuration.NumberOfWorkingWallets;
        while (iterations > 0)
        {
            var lockedWallets = await _lockManagerService.GetLocked();
            var existingWorkingAccounts = await _accountsService.GetVaultsByPrefixAndBalance(WorkingAccountNamePrefix, StellarSettings.NativeCurrencySymbol, request.Amount);
            var randomList = WalletManagerExtensions.DetermineRandomList(await _configuration.NumberOfWorkingWallets, existingWorkingAccounts, lockedWallets.Select(s => int.Parse(s.Split('-').Last())).ToList());
            foreach (var random in randomList)
            {
                var workingWallet = $"{WorkingAccountNamePrefix}-{random}";
                var lockAcquired = await _lockManagerService.AcquireLock(workingWallet);
                if (lockAcquired) return await _accountsService.GetOrCreateAccount(workingWallet);
            }
            await Task.Delay(LockDelayBetweenAttemptsMs);
            iterations--;
        }
        BeamableLogger.LogWarning("Can't find free working wallet.");
        return null;
    }

    private async Task<Account> CreateNewAndLock()
    {
        if (await _configuration.NumberOfWorkingWallets > await _configuration.MaxNumberOffWorkingWallets)
            throw new MaxNumberOfWorkingWallets();
        var nextWorkingWallet = await _configuration.NumberOfWorkingWallets + 1;
        var newWalletName = $"{WorkingAccountNamePrefix}-{nextWorkingWallet}";
        BeamableLogger.Log($"Creating new working wallet {newWalletName}.");
        await _lockManagerService.AcquireLock(newWalletName);
        await _configuration.Update(nameof(_configuration.NumberOfWorkingWallets), $"{nextWorkingWallet}");
        return await _accountsService.GetOrCreateAccount(newWalletName);
    }
}