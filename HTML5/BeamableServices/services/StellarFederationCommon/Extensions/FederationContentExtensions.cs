using StellarFederationCommon.FederationContent;

namespace StellarFederationCommon.Extensions
{
    /// <summary>
    /// FederationContentExtensions
    /// </summary>
    public static class FederationContentExtensions
    {
        /// <summary>
        /// CoinCurrency module name
        /// </summary>
        /// <param name="coinCurrency"></param>
        /// <returns></returns>
        public static string ToModuleName(this CoinCurrency coinCurrency)
            => SanitizeModuleName(coinCurrency.name).ToLowerInvariant();

        /// <summary>
        /// Removes invalid characters from a module name
        /// </summary>
        public static string SanitizeModuleName(string module)
            => module.Replace("_", "").Replace("-", "").Replace(" ", "");
    }

    /// <summary>
    /// Federation Content Type Names
    /// </summary>
    public static class FederationContentTypes
    {
        /// <summary>
        /// Regular coin type name
        /// </summary>
        public const string RegularCoinType = "coin";

        /// <summary>
        /// Crop type name
        /// </summary>
        public const string CropItemType = "crop";
    }
}