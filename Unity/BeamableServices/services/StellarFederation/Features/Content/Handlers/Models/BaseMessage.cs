using Beamable.StellarFederation.Features.Accounts.Models;

namespace Beamable.StellarFederation.Features.Content.Handlers.Models;

public abstract class BaseMessage
{
    public required string ContentId { get; set; }
    public required string ContractId { get; set; }
    public required string Function { get; set; }
    public required long GamerTag { get; set; }
    public required string PlayerWalletAddress { get; set; }
}