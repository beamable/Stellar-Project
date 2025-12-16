using System.Collections.Generic;
using StellarDotnetSdk.Responses;

namespace Beamable.StellarFederation.Features.Stellar.Models;

public class HorizonLogsResponse
{
    public List<TransactionResponse> Transactions { get; set; } = new();
    public string LastCursor { get; set; } = "";
    public uint LastProcessedLedger { get; set; }
}