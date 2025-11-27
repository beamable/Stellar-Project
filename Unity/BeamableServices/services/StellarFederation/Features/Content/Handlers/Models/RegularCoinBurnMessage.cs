namespace Beamable.StellarFederation.Features.Content.Handlers.Models;

public class RegularCoinBurnMessage : BaseMessage
{
    public required long Amount { get; set; }
}