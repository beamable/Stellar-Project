using System.Collections.Generic;
using StellarDotnetSdk.Responses.SorobanRpc;
using StellarDotnetSdk.Soroban;
using SCVal = StellarDotnetSdk.Soroban.SCVal;

namespace Beamable.StellarFederation.Features.BlockProcessor.Decoder;

public class SorobanEventDecoder<T> where T : class, new()
{
    private readonly string _eventName;

    public SorobanEventDecoder(string eventName)
    {
        _eventName = eventName;
    }

    public List<T> DecodeEvents(List<GetEventsResponse.EventInfo> events)
    {
        var decodedEvents = new List<T>();

        foreach (var eventInfo in events)
        {
            if (IsMatchingEvent(eventInfo))
            {
                var decoded = DecodeEvent(eventInfo);
                if (decoded != null)
                {
                    decodedEvents.Add(decoded);
                }
            }
        }

        return decodedEvents;
    }

    private bool IsMatchingEvent(GetEventsResponse.EventInfo eventInfo)
    {
        if (eventInfo.Topics.Length == 0)
            return false;

        var firstTopic = SCVal.FromXdrBase64(eventInfo.Topics[0]);
        return firstTopic.ToXdr().Sym.InnerValue == _eventName;
    }

    private T? DecodeEvent(GetEventsResponse.EventInfo eventInfo)
    {
        if (typeof(T) == typeof(MintEventDto))
        {
            return DecodeMintEvent(eventInfo) as T;
        }
        return null;
    }

    private MintEventDto DecodeMintEvent(GetEventsResponse.EventInfo eventInfo)
    {
        var mint = new MintEventDto();
        var walletVal = SCVal.FromXdrBase64(eventInfo.Topics[1]);
        var valueVal = SCVal.FromXdrBase64(eventInfo.Value);
        mint.Wallet = (walletVal as ScAccountId)!.InnerValue;
        mint.Amount = (long)(valueVal as SCInt128)!.Lo;
        return mint;
    }
}