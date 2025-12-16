using StellarDotnetSdk.Xdr;

namespace Beamable.StellarFederation.Features.Stellar.Extensions;

public static class ScValExtensions
{
    public static long ToInt64OrZero(this SCVal scVal)
    {
        return scVal.Discriminant.InnerValue switch
        {
            SCValType.SCValTypeEnum.SCV_U32 => scVal.U32.InnerValue,
            SCValType.SCValTypeEnum.SCV_I32 => scVal.I32.InnerValue,
            SCValType.SCValTypeEnum.SCV_U64 => (long)scVal.U64.InnerValue,
            SCValType.SCValTypeEnum.SCV_I64 => scVal.I64.InnerValue,
            SCValType.SCValTypeEnum.SCV_U128 => (long)scVal.U128.Lo.InnerValue,
            SCValType.SCValTypeEnum.SCV_I128 => (long)scVal.I128.Lo.InnerValue,
            SCValType.SCValTypeEnum.SCV_U256 => (long)scVal.U256.LoLo.InnerValue,
            SCValType.SCValTypeEnum.SCV_I256 => (long)scVal.I256.LoLo.InnerValue,
            _ => 0
        };
    }
}