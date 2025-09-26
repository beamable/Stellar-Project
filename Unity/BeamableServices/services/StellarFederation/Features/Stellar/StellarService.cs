using Beamable.StellarFederation.Features.Stellar.Models;
using StellarDotnetSdk.Accounts;

namespace Beamable.StellarFederation.Features.Stellar;

public class StellarService : IService
{
    public CreateWalletResponse CreateWallet()
    {
        var keypair = KeyPair.Random();
        return new CreateWalletResponse(keypair.Address, keypair.SecretSeed!);
    }

    public CreateWalletResponse ImportWallet(string privateKey)
    {
        var keypair = KeyPair.FromSecretSeed(privateKey);
        return new CreateWalletResponse(keypair.Address, keypair.SecretSeed!);
    }
}