using System;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using StellarFederationCommon.Extensions;
using UnityEngine;

namespace StellarFederationCommon.FederationContent
{
    /// <summary>
    /// WarriorItem NFT
    /// </summary>
    [ContentType(FederationContentTypes.WarriorItemType)]
    public class WarriorItem : ItemContent, INftBase
    {
        /// <summary>
        /// Default federation
        /// </summary>
        public WarriorItem()
        {
            federation = new OptionalFederation()
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
        [SerializeField] private SerializableDictionaryStringToString customProperties = 
            new SerializableDictionaryStringToString();
        
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
    /// WarriorItemRef
    /// </summary>
    [Serializable]
    public class WarriorItemRef : ContentRef<WarriorItem> { }
    
    /// <summary>
    /// WarriorItemLink
    /// </summary>
    [Serializable]
    public class WarriorItemLink : ContentLink<WarriorItem> { }
}