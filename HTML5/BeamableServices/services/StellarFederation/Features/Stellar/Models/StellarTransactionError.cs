using StellarDotnetSdk.Xdr;

namespace Beamable.StellarFederation.Features.Stellar.Models;

public readonly record struct StellarTransactionError(
    string Message,
    TransactionResultCode.TransactionResultCodeEnum ErrorCode)
{
    public static StellarTransactionError Empty() => default;
}