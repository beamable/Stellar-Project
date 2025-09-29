using System;
using System.Text;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Common;
using Beamable.StellarFederation.Features.HttpService;
using Beamable.StellarFederation.Features.Stellar.Models;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Assets;

namespace Beamable.StellarFederation.Features.Stellar;

public class StellarService : IService
{

    private readonly Configuration _configuration;
    private readonly HttpClientService _httpClientService;

    private readonly AssetTypeNative _nativeAsset = new();
    private StellarDotnetSdk.Server? _server;

    public StellarService(Configuration configuration, HttpClientService httpClientService)
    {
        _configuration = configuration;
        _httpClientService = httpClientService;
    }

    private async ValueTask<StellarDotnetSdk.Server> ServerInstance()
    {
        _server ??= new StellarDotnetSdk.Server(await _configuration.StellarRpc);
        return _server;
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
                var serverInstance = await ServerInstance();
                var account = await serverInstance.Accounts.Account(wallet);
                foreach (var balance in account.Balances)
                {
                    if (balance?.Asset?.Equals(_nativeAsset) ?? false)
                    {
                        result += StellarAmount.Parse(balance.BalanceString);
                    }
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