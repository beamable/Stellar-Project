namespace Beamable.StellarFederation.Features.Content.Handlers.Models;

public class RegularCoinMintMessage : BaseMessage
{
    public required long Amount { get; set; }
}