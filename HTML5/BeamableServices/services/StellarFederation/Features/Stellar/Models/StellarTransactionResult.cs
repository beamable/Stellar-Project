namespace Beamable.StellarFederation.Features.Stellar.Models;

public readonly record struct StellarTransactionResult(
    StellarTransactionStatus Status,
    string Hash,
    StellarTransactionError Error);

public enum StellarTransactionStatus
{
    Accepted,
    Failed
}