using System.Collections.Generic;
using System.Threading;
using Beamable.Common.Content;
using Cysharp.Threading.Tasks;
using Farm.Helpers;
using Farm.Managers;
using StellarFederationCommon.FederationContent;
using UnityEngine;

namespace Farm.Beam
{
    [System.Serializable]
    public class PlantInfo
    {
        public string cropName;
        public int seedsToPlant;
        public int yieldAmount;
        public int sellingPrice;
        public string contentId;
        public int instanceId;
        public CropsData cropData;
    }
    
    public class BeamContentManager : BeamManagerBase
    {
        [SerializeField] private CropItemRef[] cropItemRefs;
        
        public List<PlantInfo> Crops { get; private set; } = new List<PlantInfo>();
        public SerializableDictionary<GameConstants.CropType, PlantInfo> CropsDictionary { get; private set; } = new SerializableDictionary<GameConstants.CropType, PlantInfo>();
        
        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            await FetchCropsContent();
        }

        private async UniTask FetchCropsContent()
        {
            Crops = new List<PlantInfo>();
            CropsDictionary = new SerializableDictionary<GameConstants.CropType, PlantInfo>();
            foreach (var cropRef in cropItemRefs)
            {
                var resolvedCrop = await cropRef.Resolve();
                resolvedCrop.CustomProperties.TryGetValue(GameConstants.DataSourceProperty, out var dataSourcePath);
                resolvedCrop.CustomProperties.TryGetValue(GameConstants.SellingPriceProperty, out var sellPriceString);
                resolvedCrop.CustomProperties.TryGetValue(GameConstants.YieldHeldProperty, out var yieldHeldString);
                var plant = new PlantInfo
                {
                    cropName = resolvedCrop.Name,
                    contentId = resolvedCrop.Id,
                    instanceId = 0,
                    yieldAmount = string.IsNullOrEmpty(yieldHeldString) ? 0 : int.Parse(yieldHeldString),
                    sellingPrice = string.IsNullOrEmpty(sellPriceString) ? 0 : int.Parse(sellPriceString),
                    cropData = Resources.Load<CropsData>(dataSourcePath),
                };
                plant.seedsToPlant = plant.cropData.startingSeedsAmount;
                Crops.Add(plant);
                CropsDictionary.Add(plant.cropData.cropType, plant);
            }
        }

        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
        }
    }
}