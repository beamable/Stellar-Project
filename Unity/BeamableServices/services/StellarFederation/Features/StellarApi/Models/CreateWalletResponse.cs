namespace Beamable.StellarFederation.Features.StellarApi.Models;

public readonly record struct CreateWalletResponse(string Address, string PrivateKey, string PublicKey);