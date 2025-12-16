using System.Collections.Generic;
using StellarDotnetSdk.Responses.SorobanRpc;

namespace Beamable.StellarFederation.Features.Stellar.Models;

public class SorobanLogsResponse
{
    public List<GetEventsResponse.EventInfo> Events { get; set; } = new();
    public string LastCursor { get; set; } = "";
    public uint LastProcessedLedger { get; set; }
}