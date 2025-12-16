using System;
using Beamable.Common.Content;
using Beamable.Common.Content.Validation;
using Beamable.Common.Inventory;
using StellarFederationCommon.Extensions;
using StellarFederationCommon.FederationContent.Validation;
using UnityEngine;

namespace StellarFederationCommon.FederationContent
{
    /// <summary>
    /// Gold coin currency
    /// </summary>
    [ContentType(FederationContentTypes.GoldCoinType)]
    public class GoldCurrency : CurrencyContent, IFtBase
    {
        /// <summary>
        /// Default federation
        /// </summary>
        public GoldCurrency()
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

        [SerializeField]
        [CurrencyNameLength]
        [CannotBeBlank]
        private string _name = "";
        [SerializeField] private string _symbol = "";
        [SerializeField]
        [CurrencyDecimals]
        private int _decimals = 0;
        [SerializeField] private string _image = "";
        [SerializeField] private string _description = "";
        [MustBePositive]
        [SerializeField] private long _totalSupply = 0;

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
        /// <summary>
        /// TotalSupply
        /// </summary>
        public long TotalSupply => _totalSupply;
    }

    /// <summary>
    /// GoldCurrencyRef
    /// </summary>
    [Serializable]
    public class GoldCurrencyRef : CurrencyRef<GoldCurrency> { }

    /// <summary>
    /// GoldCurrencyLink
    /// </summary>
    [Serializable]
    public class GoldCurrencyLink : ContentLink<GoldCurrency> { }
}