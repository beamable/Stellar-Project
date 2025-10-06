using System;
using System.Text;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.HttpService;
using Beamable.StellarFederation.Features.Stellar.Models;
using StellarDotnetSdk;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.LedgerEntries;
using StellarDotnetSdk.Soroban;
using LedgerKey = StellarDotnetSdk.LedgerKeys.LedgerKey;

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

    public bool IsSignatureValid(string wallet, string message, string signature)
    {
        var keypair = KeyPair.FromAccountId(wallet);
        return keypair.Verify(
            Encoding.UTF8.GetBytes(message),
            Convert.FromBase64String(signature)
        );
    }

    public string Sign(string secretSeed, string message)
    {
        var keypair = KeyPair.FromSecretSeed(secretSeed);
        var signature = keypair.Sign(
            Encoding.UTF8.GetBytes(message)
        );
        return Convert.ToBase64String(signature);
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
}