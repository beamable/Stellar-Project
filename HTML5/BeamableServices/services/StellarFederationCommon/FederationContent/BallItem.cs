using System;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using StellarFederationCommon.Extensions;
using UnityEngine;

namespace StellarFederationCommon.FederationContent
{
    
    /// <summary>
    /// BallItem NFT
    /// </summary>
    [ContentType(FederationContentTypes.BallItemType)]
    public class BallItem : ItemContent, INftBase
    {
        /// <summary>
        /// Default federation
        /// </summary>
        public BallItem()
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

        [SerializeField] private string name = "";
        [SerializeField] private string description = "";
        [SerializeField] private string image = "";
        [SerializeField] private SerializableDictionaryStringToString customProperties = new SerializableDictionaryStringToString();
        
        /// <summary>
        /// Token name
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Token description
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Token image
        /// </summary>
        public string Image => image;

        /// <summary>
        /// Token custom properties
        /// </summary>
        public SerializableDictionaryStringToString CustomProperties => customProperties;
 
    }

    /// <summary>
    /// BallItemRef
    /// </summary>
    [Serializable]
    public class BallItemRef : ContentRef<BallItem>
    {
    }

    /// <summary>
    /// BallItemLink
    /// </summary>
    [Serializable]
    public class BallItemLink : ContentLink<BallItem>
    {
    }
}