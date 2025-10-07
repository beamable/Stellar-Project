using System;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using StellarFederationCommon.Extensions;
using UnityEngine;

namespace StellarFederationCommon.FederationContent
{
    
    /// <summary>
    /// CropType
    /// </summary>
    public enum CropType{ Carrot, Tomato, Pumpkin}
    
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

        [SerializeField] private string name = "";
        [SerializeField] private string description = "";
        [SerializeField] private string _image = "";
        [SerializeField] private SerializableDictionaryStringToString customProperties = new SerializableDictionaryStringToString();

        [Header("Crop Details")] 
        [SerializeField] private int startingSeedAmount;
        [SerializeField] private int yield;
        [SerializeField] private int sellingValue;
        [SerializeField] private CropType cropType;
        
        [Header("Crop Sprites")]
        [SerializeField] private Sprite cropIcon;
        [SerializeField] private Sprite seedSprite;
        [SerializeField] private Sprite cropPlantedSprite, cropWateredSprite, cropGrowingSprite, cropRipeSprite;
        
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
        public string Image => _image;

        /// <summary>
        /// Token custom properties
        /// </summary>
        public SerializableDictionaryStringToString CustomProperties => customProperties;
 
        /// <summary>
        /// Starting seed amount
        /// </summary>
        public int StartingSeedAmount => startingSeedAmount;

        /// <summary>
        /// Yield
        /// </summary>
        public int Yield => yield;
        
        /// <summary>
        /// Selling Value
        /// </summary>
        public int SellingValue => sellingValue;
        
        /// <summary>
        /// Crop Type
        /// </summary>
        public CropType Type => cropType;
        
        /// <summary>
        /// Crop icon
        /// </summary>
        public Sprite CropIcon => cropIcon;
        /// <summary>
        /// Seed sprite
        /// </summary>
        public Sprite SeedSprite => seedSprite;
        /// <summary>
        /// CropPlantedSprite
        /// </summary>
        public Sprite CropPlantedSprite => cropPlantedSprite;
        /// <summary>
        /// CropWateredSprite
        /// </summary>
        public Sprite CropWateredSprite => cropWateredSprite;
        /// <summary>
        /// CropGrowingSprite
        /// </summary>
        public Sprite CropGrowingSprite => cropGrowingSprite;
        /// <summary>
        /// CropRipeSprite
        /// </summary>
        public Sprite CropRipeSprite => cropRipeSprite;
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