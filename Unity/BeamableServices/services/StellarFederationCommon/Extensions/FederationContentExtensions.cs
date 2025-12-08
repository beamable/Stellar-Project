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


        /// <summary>
        /// ItemContent name (items.part)
        /// </summary>
        /// <param name="itemContent"></param>
        /// <returns></returns>
        public static string ToItemNameType(this IContentObject itemContent)
        {
            if (string.IsNullOrEmpty(itemContent.Id))
                return "";

            var firstDot = itemContent.Id.IndexOf('.');
            if (firstDot == -1)
                return itemContent.Id;

            var secondDot = itemContent.Id.IndexOf('.', firstDot + 1);
            return secondDot == -1 ? itemContent.Id : itemContent.Id[..secondDot];
        }

        /// <summary>
        /// ItemContent type (items."part")
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public static string ToItemNameType(this string contentId)
        {
            if (string.IsNullOrEmpty(contentId))
                return contentId;

            var firstDot = contentId.IndexOf('.');
            if (firstDot == -1)
                return string.Empty;

            var secondDot = contentId.IndexOf('.', firstDot + 1);
            if (secondDot == -1)
                return contentId.Substring(firstDot + 1); // only two parts

            return SanitizeModuleName(contentId.Substring(firstDot + 1, secondDot - firstDot - 1));
        }

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