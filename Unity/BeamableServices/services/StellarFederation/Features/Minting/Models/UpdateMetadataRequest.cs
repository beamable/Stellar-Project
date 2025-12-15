namespace Beamable.StellarFederation.Features.Minting.Models;

public class UpdateMetadataRequest
{
    public uint TokenId { get; set; }
    public NftExternalMetadata Metadata { get; set; } = null!;

    public string MetadataHash { get; set; } = "";
}