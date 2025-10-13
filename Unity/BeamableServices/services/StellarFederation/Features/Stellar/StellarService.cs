using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Accounts;
using Beamable.StellarFederation.Features.Accounts.Exceptions;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.HttpService;
using Beamable.StellarFederation.Features.Stellar.Exceptions;
using Beamable.StellarFederation.Features.Stellar.Models;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Federation;
using StellarDotnetSdk.LedgerEntries;
using StellarDotnetSdk.Memos;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Soroban;
using StellarDotnetSdk.Transactions;
using StellarDotnetSdk.Xdr;
using LedgerKey = StellarDotnetSdk.LedgerKeys.LedgerKey;
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
    private const int FaucetWaitTimeSec = 20;

    private SorobanServer? _rpcServer;

    public StellarService(Configuration configuration, HttpClientService httpClientService, AccountsService accountsService, StellarTransactionBuilderFactory transactionBuilderFactory)
    {
        _configuration = configuration;
        _httpClientService = httpClientService;
        _accountsService = accountsService;
        _transactionBuilderFactory = transactionBuilderFactory;
    }

    private async ValueTask<SorobanServer> RpcInstance()
    {
        await SetNetwork();
        _rpcServer ??= new SorobanServer(await _configuration.StellarRpc);
        return _rpcServer;
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

    public async Task<StellarAmount> NativeBalance(string wallet)
    {
        var result = StellarAmount.NativeZero;
        using (new Measure(nameof(NativeBalance)))
        {
            try
            {
                var serverInstance = await RpcInstance();
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
                await RpcInstance();
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
                await RpcInstance();
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

    private async Task<Account?> GetStellarAccount(string wallet)
    {
        using (new Measure(nameof(GetStellarAccount)))
        {
            try
            {
                var rpcInstance = await RpcInstance();
                return await rpcInstance.GetAccount(wallet);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogWarning("Account {address} does not exist on the ledger: {error}", wallet, ex.Message);
                return null;
            }
        }
    }

    public async Task<StellarTransactionResult> TransferNative(string toAddress, long amount)
    {
        using (new Measure(nameof(TransferNative)))
        {
            try
            {
                var rpcInstance = await RpcInstance();
                var realmAccount = await _accountsService.GetOrCreateRealmAccount();
                var serverKeyPair = KeyPair.FromSecretSeed(realmAccount.SecretSeed);
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
                transaction.Sign(serverKeyPair);
                var response = await rpcInstance.SendTransaction(transaction);
                return new StellarTransactionResult(response.Status.ToStellarStatus(), response.Hash, response.GetErrorMessage());
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Can't Transfer to {address}. Error: {error}", toAddress, ex.Message);
                throw new StellarServiceException($"Transfer: {ex.Message}");
            }
        }
    }

    public async Task Initialize()
    {
        if (!_initialized)
        {
            try
            {
                var realmAccount = await _accountsService.GetOrCreateRealmAccount();
                await RpcInstance();
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
            var server = await RpcInstance();
            var contractAddress = new ScContractId(address);

            var xdrInstanceKey = new StellarDotnetSdk.Xdr.SCVal
            {
                Discriminant = SCValType.Create(SCValType.SCValTypeEnum.SCV_LEDGER_KEY_CONTRACT_INSTANCE)
            };
            var instanceKey = StellarDotnetSdk.Soroban.SCVal.FromXdr(xdrInstanceKey);
            var durability = ContractDataDurability.Create(ContractDataDurability.ContractDataDurabilityEnum.PERSISTENT);
            var ledgerKey = LedgerKey.ContractData(contractAddress, instanceKey, durability);
            await server.GetLedgerEntry(ledgerKey);
            return true;
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
}