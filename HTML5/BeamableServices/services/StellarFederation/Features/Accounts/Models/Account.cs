using StellarDotnetSdk.Accounts;

namespace Beamable.StellarFederation.Features.Accounts.Models;

public readonly record struct Account(string Name, string Address, string SecretSeed, bool Created)
{
    public KeyPair KeyPair => KeyPair.FromSecretSeed(SecretSeed);
}