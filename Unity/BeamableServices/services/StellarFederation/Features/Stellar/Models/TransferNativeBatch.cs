namespace Beamable.StellarFederation.Features.Stellar.Models;

public record TransferNativeBatch
{
    public required string ToAddress { get; set; }
    public long Amount { get; set; }
}