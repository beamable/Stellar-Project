using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Accounts.Exceptions;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.HttpService;
using Beamable.StellarFederation.Features.Stellar.Models;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.LedgerEntries;
using StellarDotnetSdk.Memos;
using StellarDotnetSdk.Operations;
using StellarDotnetSdk.Soroban;
using StellarDotnetSdk.Transactions;
using LedgerKey = StellarDotnetSdk.LedgerKeys.LedgerKey;
using TimeBounds = StellarDotnetSdk.Transactions.TimeBounds;
using Transaction = StellarDotnetSdk.Transactions.Transaction;

namespace Beamable.StellarFederation.Features.Stellar;

public class StellarService : IService
{

    private readonly Configuration _configuration;
    private readonly HttpClientService _httpClientService;

    private SorobanServer? _rpcServer;

    public StellarService(Configuration configuration, HttpClientService httpClientService)
    {
        _configuration = configuration;
        _httpClientService = httpClientService;
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

    public CreateWalletResponse CreateWallet()
    {
        var keypair = KeyPair.Random();
        return new CreateWalletResponse(keypair.Address, keypair.AccountId, keypair.SecretSeed!);
    }

    public CreateWalletResponse ImportWallet(string secretSeed)
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
}