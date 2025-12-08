using System;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using StellarFederationCommon.Extensions;
using UnityEngine;

namespace StellarFederationCommon.FederationContent
{
    
    /// <summary>
    /// CropItem NFT
    /// </summary>
    [ContentType(FederationContentTypes.CropItemType)]
    public class CropItem : ItemContent, INftBase
    {
        /// <summary>
        /// Default federation
        /// </summary>
        public CropItem()
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

        [SerializeField] private new string name = "";
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
    /// CropItemRef
    /// </summary>
    [Serializable]
    public class CropItemRef : ContentRef<CropItem>
    {
    }

    /// <summary>
    /// CropItemLink
    /// </summary>
    [Serializable]
    public class CropItemLink : ContentLink<CropItem>
    {
    }
}