using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Accounts.Exceptions;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.HttpService;
using Beamable.StellarFederation.Features.Scheduler.Storage.Modles;
using Beamable.StellarFederation.Features.Stellar.Exceptions;
using Beamable.StellarFederation.Features.Stellar.Extensions;
using Beamable.StellarFederation.Features.Stellar.Models;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Federation;
using StellarDotnetSdk.LedgerEntries;
using StellarDotnetSdk.LedgerKeys;
using StellarDotnetSdk.Memos;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Requests;
using StellarDotnetSdk.Requests.SorobanRpc;
using StellarDotnetSdk.Responses;
using StellarDotnetSdk.Responses.SorobanRpc;
using StellarDotnetSdk.Soroban;
using StellarDotnetSdk.Transactions;
using StellarDotnetSdk.Xdr;
using LedgerKey = StellarDotnetSdk.LedgerKeys.LedgerKey;
using SCVal = StellarDotnetSdk.Soroban.SCVal;
using SCVec = StellarDotnetSdk.Soroban.SCVec;
using TimeBounds = StellarDotnetSdk.Transactions.TimeBounds;
using Transaction = StellarDotnetSdk.Transactions.Transaction;

namespace Beamable.StellarFederation.Features.Stellar;

public class StellarService : IService
{

    private readonly Configuration _configuration;
    private readonly HttpClientService _httpClientService;
    private readonly AccountsService _accountsService;
    private readonly StellarTransactionBuilderFactory _transactionBuilderFactory;

    private bool _initialized;
    private const int FaucetWaitTimeSec = 10;
    private const int LogsLimit = 200;

    private SorobanServer? _rpcServer;
    private StellarDotnetSdk.Server? _horizonServer;

    public StellarService(Configuration configuration, HttpClientService httpClientService, AccountsService accountsService, StellarTransactionBuilderFactory transactionBuilderFactory)
    {
        _configuration = configuration;
        _httpClientService = httpClientService;
        _accountsService = accountsService;
        _transactionBuilderFactory = transactionBuilderFactory;
    }

    private async ValueTask<SorobanServer> SorobanInstance()
    {
        await SetNetwork();
        _rpcServer ??= new SorobanServer(await _configuration.StellarRpc);
        return _rpcServer;
    }

    private async ValueTask<StellarDotnetSdk.Server> HorizonInstance()
    {
        await SetNetwork();
        _horizonServer ??= new StellarDotnetSdk.Server(await _configuration.StellarHorizon);
        return _horizonServer;
    }

    private async ValueTask SetNetwork()
    {
        if (Network.Current is null || (Network.IsPublicNetwork(Network.Current) && await _configuration.StellarNetwork == StellarSettings.TestNetwork))
        {
            if (await _configuration.StellarNetwork == StellarSettings.TestNetwork)
                Network.UseTestNetwork();
            else
                Network.UsePublicNetwork();
        }
    }

    public async ValueTask<Hash> GetNetworkHash()
    {
        await SetNetwork();
        return new Hash(Network.Current!.NetworkId);
    }

    public static CreateWalletResponse CreateWallet()
    {
        var keypair = KeyPair.Random();
        return new CreateWalletResponse(keypair.Address, keypair.AccountId, keypair.SecretSeed!);
    }

    public static CreateWalletResponse ImportWallet(string secretSeed)
    {
        var keypair = KeyPair.FromSecretSeed(secretSeed);
        return new CreateWalletResponse(keypair.Address, keypair.AccountId, keypair.SecretSeed!);
    }

    public bool IsSignatureValid(string wallet, string message, string signature, string messagePrefix = "")
    {
        try
        {
            var preimageBytes = Encoding.UTF8.GetBytes($"{messagePrefix}{message}");
            var digest = SHA256.HashData(preimageBytes);
            var sigBytes = Convert.FromBase64String(signature);
            var kp = KeyPair.FromAccountId(wallet);
            return kp.Verify(digest, sigBytes);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task TryRequestFaucet(string wallet)
    {
        using (new Measure(nameof(TryRequestFaucet)))
        {
            try
            {
                await _httpClientService.Get(await _configuration.StellarFaucet + "?addr=" + wallet);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Faucet request failed for wallet: {wallet} with error: {error}", wallet,
                    ex.Message);
            }
        }
    }

    public async Task<uint> GetCurrentLedgerSequence()
    {
        using (new Measure(nameof(GetCurrentLedgerSequence)))
        {
            try
            {
                var serverInstance = await SorobanInstance();
                var latest = await serverInstance.GetLatestLedger();
                return (uint)latest.Sequence;
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("GetLatestLedger failed with error: {error}", ex.Message);
                throw new StellarServiceException($"GetLatestLedger failed with error: {ex.Message}");
            }
        }
    }

    public async Task<StellarAmount> NativeBalance(string wallet)
    {
        var result = StellarAmount.NativeZero;
        using (new Measure(nameof(NativeBalance)))
        {
            try
            {
                var serverInstance = await SorobanInstance();
                var accountKey = LedgerKey.Account(KeyPair.FromAccountId(wallet));
                var response = await serverInstance.GetLedgerEntry(accountKey);

                if (response.LedgerEntries is not { Length: > 0 }) return result;

                foreach (var entry in response.LedgerEntries)
                {
                    var entryAccount = (LedgerEntryAccount)entry;
                    result += entryAccount.Balance;
                }
                return result;
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Balance request failed for wallet: {wallet} with error: {error}", wallet,
                    ex.Message);
                return StellarAmount.NativeZero;
            }
        }
    }

    public async Task<string> CreateSignTransaction(Beamable.StellarFederation.Features.Accounts.Models.Account realAccount, string wallet)
    {
        using (new Measure(nameof(CreateSignTransaction)))
        {
            try
            {
                await SorobanInstance();
                var dummyAccount = new Account(realAccount.Address, 0);
                var serverKeypair = KeyPair.FromSecretSeed(realAccount.SecretSeed);
                var userKeypair = KeyPair.FromAccountId(wallet);
                var authOperation = new ManageDataOperation("auth-challenge", "Sign to prove ownership", userKeypair);
                var transaction = new TransactionBuilder(dummyAccount)
                    .AddOperation(authOperation)
                    .SetFee(100)
                    .AddMemo(new MemoText("auth-challenge"))
                    .AddTimeBounds(new TimeBounds(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5)))
                    .Build();
                transaction.Sign(serverKeypair);
                return transaction.ToEnvelopeXdrBase64();
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Failed creating sign transaction for wallet: {wallet} with error: {error}", wallet,
                    ex.Message);
                throw new AuthenticateException("Failed creating sign transaction");
            }
        }
    }


    public async Task<bool> IsSignTransactionValid(Beamable.StellarFederation.Features.Accounts.Models.Account realAccount, string wallet, string solution)
    {
        using (new Measure(nameof(IsSignTransactionValid)))
        {
            try
            {
                await SorobanInstance();
                var userKeyPair = KeyPair.FromAccountId(wallet);
                var serverKeyPair = KeyPair.FromSecretSeed(realAccount.SecretSeed);

                var tx = Transaction.FromEnvelopeXdr(solution);

                var txHash = tx.Hash(Network.Current!);

                // Verify server and user signatures against this hash
                var serverValid = tx.Signatures.Any(sig => serverKeyPair.Verify(txHash, sig.Signature.InnerValue));
                var userValid   = tx.Signatures.Any(sig => userKeyPair.Verify(txHash, sig.Signature.InnerValue));

                return serverValid && userValid;
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Failed to verify sign transaction for wallet: {wallet} with error: {error}", wallet,
                    ex.Message);
                throw new AuthenticateException("Failed to verify sign transaction");
            }
        }
    }

    public async Task<Account?> GetStellarAccount(string wallet)
    {
        using (new Measure(nameof(GetStellarAccount)))
        {
            try
            {
                var rpcInstance = await SorobanInstance();
                return await rpcInstance.GetAccount(wallet);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogWarning("Account {address} does not exist on the ledger: {error}", wallet, ex.Message);
                return null;
            }
        }
    }

    public async Task<Account> GetRealmStellarAccount(string wallet)
    {
        using (new Measure(nameof(GetRealmStellarAccount)))
        {
            try
            {
                var rpcInstance = await SorobanInstance();
                return await rpcInstance.GetAccount(wallet);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogWarning("Realm account {address} does not exist on the ledger: {error}", wallet, ex.Message);
                throw new UnknownAccountException($"Realm account {wallet} does not exist on the ledger.");
            }
        }
    }

    public async Task<GetTransactionResponse?> GetStellarTransaction(string hash)
    {
        using (new Measure(nameof(GetStellarTransaction)))
        {
            try
            {
                var rpcInstance = await SorobanInstance();
                return await rpcInstance.GetTransaction(hash);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogWarning("Transaction {hash} does not exist on the ledger: {error}", hash, ex.Message);
                return null;
            }
        }
    }

    public async Task<TransactionBuilder> CreateDefaultNativeBuilder(Account sourceAccount, string memo)
    {
        return await _transactionBuilderFactory.CreateDefaultBuilder(sourceAccount, memo);
    }

    public async Task<StellarTransactionResult> TransferNative(string toAddress, long amount)
    {
        using (new Measure(nameof(TransferNative)))
        {
            try
            {
                var rpcInstance = await SorobanInstance();
                var realmAccount = await _accountsService.GetOrCreateRealmAccount();
                var destinationKeyPair = KeyPair.FromAccountId(toAddress);
                var sourceAccount = await GetStellarAccount(realmAccount.Address);

                var transactionBuilder = await _transactionBuilderFactory.CreateDefaultBuilder(sourceAccount!, nameof(TransferNative));

                var transferAmount = new StellarAmount(amount);

                var destinationAccount = await GetStellarAccount(toAddress);
                if (destinationAccount is null)
                {
                    transactionBuilder
                        .AddCreateAccountOperation(destinationKeyPair, transferAmount);
                }
                else
                {
                    transactionBuilder
                        .AddNativeTransferOperation(destinationKeyPair, transferAmount);
                }

                var transaction = transactionBuilder.Build();
                transaction.Sign(realmAccount.KeyPair);
                var response = await rpcInstance.SendTransaction(transaction);

                return response.ToStellarTransactionResult();
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Can't Transfer to {address}. Error: {error}", toAddress, ex.Message);
                throw new StellarServiceException($"Transfer: {ex.Message}");
            }
        }
    }

    public async Task<StellarTransactionResult> TransferNativeBatch(List<TransferNativeBatch> nativeBatches)
    {
        using (new Measure(nameof(TransferNativeBatch)))
        {
            try
            {
                var rpcInstance = await SorobanInstance();
                var realmAccount = await _accountsService.GetOrCreateRealmAccount();
                var sourceAccount = await GetStellarAccount(realmAccount.Address);
                var transactionBuilder = await _transactionBuilderFactory.CreateDefaultBuilder(sourceAccount!, nameof(TransferNativeBatch));

                foreach (var batch in nativeBatches)
                {
                    var destinationKeyPair = KeyPair.FromAccountId(batch.ToAddress);
                    var transferAmount = new StellarAmount(batch.Amount);
                    var destinationAccount = await GetStellarAccount(batch.ToAddress);
                    if (destinationAccount is null)
                    {
                        transactionBuilder
                            .AddCreateAccountOperation(destinationKeyPair, transferAmount);
                    }
                    else
                    {
                        transactionBuilder
                            .AddNativeTransferOperation(destinationKeyPair, transferAmount);
                    }
                }

                var transaction = transactionBuilder.Build();
                transaction.Sign(realmAccount.KeyPair);
                var response = await rpcInstance.SendTransaction(transaction);
                return response.ToStellarTransactionResult();
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Can't execute TransferNativeBatch. Error: {error}", ex.Message);
                throw new StellarServiceException($"TransferNativeBatch: {ex.Message}");
            }
        }
    }

    public async Task<SimulateTransactionResponse> SimulateTransaction(Transaction transaction)
    {
        using (new Measure(nameof(SimulateTransaction)))
        {
            var rpcInstance = await SorobanInstance();
            return await rpcInstance.SimulateTransaction(transaction);
        }
    }

    public async Task<SendTransactionResponse> SendTransaction(Transaction transaction)
    {
        using (new Measure(nameof(SendTransaction)))
        {
            var rpcInstance = await SorobanInstance();
            return await rpcInstance.SendTransaction(transaction);
        }
    }

    public async Task Initialize()
    {
        if (!_initialized)
        {
            try
            {
                var realmAccount = await _accountsService.GetOrCreateRealmAccount();
                await SorobanInstance();
                var network = await _configuration.StellarNetwork;
                _initialized = true;

                if (network is StellarSettings.TestNetwork)
                {
                    var balance = await NativeBalance(realmAccount.Address);
                    if (balance == StellarAmount.NativeZero)
                    {
                        BeamableLogger.Log($"Requesting faucet coins, waiting {FaucetWaitTimeSec} sec...");
                        await TryRequestFaucet(realmAccount.Address);
                        await Task.Delay(TimeSpan.FromSeconds(FaucetWaitTimeSec));
                        BeamableLogger.Log("Done requesting faucet coins.");
                    }
                }
            }
            catch (Exception e)
            {
                throw new StellarServiceException($"Failed to initialize: {e.Message}");
            }
        }
    }

    public async Task<bool> ContractExists(string address)
    {
        try
        {
            var server = await SorobanInstance();
            var contractId = new ScContractId(address);
            var durability = ContractDataDurability.Create(ContractDataDurability.ContractDataDurabilityEnum.PERSISTENT);
            var ledgerKeyContractData = new LedgerKeyContractData(
                contractId,
                new SCLedgerKeyContractInstance(),
                durability
            );
            var contractCodeResponse = await server.GetLedgerEntry(ledgerKeyContractData);
            return contractCodeResponse.LedgerEntries is not null && contractCodeResponse.LedgerEntries.Length != 0;
        }
        catch (NotFoundException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<HorizonLogsResponse> GetHorizonLogs(string accountAddress, Block block)
    {
        using (new Measure(nameof(GetHorizonLogs)))
        {
            var instance = await HorizonInstance();
            var result = new HorizonLogsResponse
            {
                LastCursor = block.Cursor,
                LastProcessedLedger = block.BlockNumber
            };
            var requestBuilder = instance.Transactions
                .ForAccount(accountAddress)
                .Order(OrderDirection.ASC)
                .Limit(LogsLimit);

            if (!string.IsNullOrWhiteSpace(block.Cursor))
            {
                requestBuilder = requestBuilder.Cursor(block.Cursor);
            }

            var page = await requestBuilder.Execute();

            foreach (var tx in page.Records)
            {
                result.LastCursor = tx.PagingToken;
                result.LastProcessedLedger = tx.Ledger;
                result.Transactions.Add(tx);
            }

            return result;
        }
    }

    public async Task<SorobanLogsResponse> GetSorobanLogs(Block block, string[] contractIds)
    {
        using (new Measure(nameof(GetSorobanLogs)))
        {
            var rpcInstance = await SorobanInstance();
            var result = new SorobanLogsResponse
            {
                LastCursor = block.Cursor,
                LastProcessedLedger = block.BlockNumber
            };
            var eventsResponse = await rpcInstance.GetEvents(new GetEventsRequest
            {
                StartLedger = block.BlockNumber,
                Filters =
                [
                    new GetEventsRequest.EventFilter
                    {
                        ContractIds = contractIds
                    }
                ],
                Pagination = new PaginationOptions
                {
                    Limit = LogsLimit,
                    Cursor = !string.IsNullOrWhiteSpace(block.Cursor) ? block.Cursor : null
                }
            });
            if (eventsResponse.Events is not null)
            {
                foreach (var eventInfo in eventsResponse.Events)
                {
                    result.Events.Add(eventInfo);
                    result.LastProcessedLedger = (uint)eventInfo.Ledger;
                }

                if (eventsResponse.Events.Length > LogsLimit)
                {
                    result.LastCursor = eventsResponse.Cursor ?? "";
                }
                else
                {
                    result.LastProcessedLedger = (uint)eventsResponse.LatestLedger.GetValueOrDefault(0);
                }
            }

            return result;
        }
    }
}