using Beamable.Common.Content;

namespace StellarFederationCommon.FederationContent
{
    /// <summary>
    /// All NFT content types should implement this interface.
    /// </summary>
    public interface INftBase
    {
        /// <summary>
        /// Token name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Token description
        /// </summary>
        string Image { get; }
        /// <summary>
        /// Token image
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Token custom properties
        /// </summary>
        SerializableDictionaryStringToString CustomProperties { get; }
    }
}