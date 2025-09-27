using System;
using System.Text;
using Beamable.StellarFederation.Features.Stellar.Models;
using StellarDotnetSdk.Accounts;

namespace Beamable.StellarFederation.Features.Stellar;

public class StellarService : IService
{
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
}