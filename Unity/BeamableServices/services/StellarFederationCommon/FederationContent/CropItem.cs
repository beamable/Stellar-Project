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

        [SerializeField] private readonly string _name = "";
        [SerializeField] private readonly string _description = "";
        [SerializeField] private readonly string _image = "";
        [SerializeField] private readonly SerializableDictionaryStringToString _customProperties = new SerializableDictionaryStringToString();

        /// <summary>
        /// Token name
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Token description
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// Token image
        /// </summary>
        public string Image => _image;

        /// <summary>
        /// Token custom properties
        /// </summary>
        public SerializableDictionaryStringToString CustomProperties => _customProperties;
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