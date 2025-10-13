using System;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using StellarFederationCommon.Extensions;
using UnityEngine;

namespace StellarFederationCommon.FederationContent
{
    /// <summary>
    /// Regular coin currency
    /// </summary>
    [ContentType(FederationContentTypes.RegularCoinType)]
    public class CoinCurrency : CurrencyContent, IFtBase
    {
        /// <summary>
        /// Default federation
        /// </summary>
        public CoinCurrency()
        {
            federation = new OptionalFederation
            {
                HasValue = true,
                Value = new Federation
                {
                    Service = StellarFederationSettings.MicroserviceName,
                    Namespace = StellarFederationSettings.StellarIdentityName
                }
            };
        }

        [SerializeField] private string _name = "";
        [SerializeField] private string _symbol = "";
        [SerializeField] private int _decimals = 0;
        [SerializeField] private string _image = "";
        [SerializeField] private string _description = "";

        /// <summary>
        /// name
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// symbol
        /// </summary>
        public string Symbol => _symbol;
        /// <summary>
        /// decimals
        /// </summary>
        public int Decimals => _decimals;
        /// <summary>
        /// image
        /// </summary>
        public string Image => _image;
        /// <summary>
        /// description
        /// </summary>
        public string Description => _description;
    }

    /// <summary>
    /// CoinCurrencyRef
    /// </summary>
    [Serializable]
    public class CoinCurrencyRef : CurrencyRef<CoinCurrency> { }

    /// <summary>
    /// CoinCurrencyLink
    /// </summary>
    [Serializable]
    public class CoinCurrencyLink : ContentLink<CoinCurrency> { }
}