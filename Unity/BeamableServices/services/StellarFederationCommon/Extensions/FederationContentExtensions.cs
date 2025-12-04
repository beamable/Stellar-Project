using Beamable.Common.Content;
using Beamable.Common.Inventory;

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
        public static string ToCurrencyModuleName(this CurrencyContent coinCurrency)
            => SanitizeModuleName(GetLastPartAfterDot(coinCurrency.Id)).ToLowerInvariant();

        private static string GetLastPartAfterDot(this string contentId)
            => contentId.Contains('.') ? contentId[(contentId.LastIndexOf('.') + 1)..] : contentId;

        /// <summary>
        /// CoinCurrency module name
        /// </summary>
        /// <param name="coinCurrencyContentId"></param>
        /// <returns></returns>
        public static string ToCurrencyModuleName(this string coinCurrencyContentId)
            => SanitizeModuleName(GetLastPartAfterDot(coinCurrencyContentId)).ToLowerInvariant();

        private static string ToContentType(this string contentId)
            => contentId.Contains('.') ? contentId[..contentId.LastIndexOf('.')] : contentId;

        /// <summary>
        /// Item content type from string
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public static string ToItemModuleName(this string contentId)
            => SanitizeModuleName(ToContentType(contentId)).ToLowerInvariant();

        /// <summary>
        /// Content type
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public static string ExtractMiddle(this string contentId)
        {
            var parts = contentId.Split('.');
            return parts.Length >= 3 ? parts[1] : "";
        }

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
        /// Gold coin type name
        /// </summary>
        public const string GoldCoinType = "gold";

        /// <summary>
        /// Crop type name
        /// </summary>
        public const string CropItemType = "crop";
    }
}