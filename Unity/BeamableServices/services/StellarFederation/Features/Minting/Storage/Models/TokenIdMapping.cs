namespace Beamable.StellarFederation.Features.Minting.Storage.Models;

public record TokenIdMapping
{
    public string ContentId { get; set; } = null!;
    public uint TokenId { get; set; }
    public string MetadataHash { get; set; }
}