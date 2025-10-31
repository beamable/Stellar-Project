using StellarDotnetSdk.Responses.SorobanRpc;

namespace Beamable.StellarFederation.Features.Stellar.Models;

public readonly record struct StellarTransactionResult(
    StellarTransactionStatus Status,
    string Hash,
    string Error);

public enum StellarTransactionStatus
{
    Accepted,
    Failed
}

public static class StellarTransactionResultExtensions
{
    public static StellarTransactionStatus ToStellarStatus(this SendTransactionResponse.SendTransactionStatus sendStatus)
    {
        return sendStatus == SendTransactionResponse.SendTransactionStatus.PENDING
            ? StellarTransactionStatus.Accepted
            : StellarTransactionStatus.Failed;
    }
}