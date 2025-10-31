namespace Beamable.StellarFederation.Features.WalletManager.Models;

public readonly record struct StellarLockRequest(long Amount)
{
    public static StellarLockRequest Default() => new(10_000_000);
}