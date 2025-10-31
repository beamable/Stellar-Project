namespace StellarFederationCommon.FederationContent
{
    /// <summary>
    /// All FT content types should implement this interface.
    /// </summary>
    public interface IFtBase
    {
        /// <summary>
        ///
        /// </summary>
        string Name { get; }
        /// <summary>
        ///
        /// </summary>
        string Symbol { get; }
        /// <summary>
        ///
        /// </summary>
        int Decimals { get; }
        /// <summary>
        ///
        /// </summary>
        string Image { get; }
        /// <summary>
        ///
        /// </summary>
        string Description { get; }
    }
}