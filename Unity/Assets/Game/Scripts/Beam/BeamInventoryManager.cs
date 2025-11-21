using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Beamable;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Player;
using Cysharp.Threading.Tasks;
using Farm.Helpers;
using StellarFederationCommon.FederationContent;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamInventoryManager : BeamManagerBase
    {
        [SerializeField] private CropItemRef defaultCropRef;

        private BeamContentManager _contentManager;
        
        public List<PlantInfo> PlayerCrops { get; private set; }
        public Dictionary<long, PlantInfo> CropInstancesDictionary { get; private set; } = new Dictionary<long, PlantInfo>();
        
        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            _contentManager = BeamManager.Instance.ContentManager;
            PlayerCrops = new List<PlantInfo>();
            await FetchInventory();
        }

        public async UniTask FetchInventory()
        {
            await _beamContext.Inventory.Refresh();
            //default crop
            var inv = await _beamContext.Api.InventoryService.GetCurrent();
            await UpdateDefaultCropInfo(inv);
        }

        private async UniTask<bool> UpdateDefaultCropInfo(InventoryView itemsGroup)
        {
            var defaultCropInfo = _contentManager.GetCropInfo(defaultCropRef.Id);
            if (itemsGroup.items.Count < 1 || !itemsGroup.items.ContainsKey(defaultCropRef.Id))
            {
                var properties = new Dictionary<string, string>
                {
                    { GameConstants.SeedsLeftProp, defaultCropInfo.seedsToPlant.ToString() },
                    { GameConstants.YieldProp, defaultCropInfo.yieldAmount.ToString() }
                };
                await _stellarClient.AddItem(defaultCropRef.Id, properties);
                return false;
            }

            var cropInstances = itemsGroup.items[defaultCropRef.Id];
            var cropInstance = cropInstances[0];

            defaultCropInfo.instanceId = cropInstance.id;
            defaultCropInfo.yieldAmount =
                cropInstance.properties.TryGetValue(GameConstants.YieldProp, out var yieldAmount)
                    ? int.Parse(yieldAmount)
                    : 0;
            defaultCropInfo.seedsToPlant =
                cropInstance.properties.TryGetValue(GameConstants.SeedsLeftProp, out var seedsAmount)
                    ? int.Parse(seedsAmount)
                    : 0;

            var defaultPlayerCrop = TryGetCropInfoByInstanceId(defaultCropInfo.instanceId);
            if (defaultPlayerCrop == null)
            {
                CropInstancesDictionary.Add(defaultCropInfo.instanceId, defaultCropInfo);
                PlayerCrops.Add(defaultCropInfo);
            }
            else
            {
                CropInstancesDictionary[cropInstance.id] = defaultCropInfo;
                var index = PlayerCrops.FindIndex(x => x.instanceId == cropInstance.id);
                PlayerCrops[index] = defaultCropInfo;
            }

            return true;
        }

        public async UniTask UpdateCropInfos()
        {
            if(PlayerCrops.Count < 1) return;
            try
            {
                var itemsToUpdate = new List<CropUpdateRequest>();

                foreach (var crop in PlayerCrops)
                {
                    var itemToUpdate = new CropUpdateRequest()
                    {
                        ContentId = crop.contentId,
                        InstanceId = crop.instanceId,
                        Properties = new Dictionary<string, string>()
                        {
                            {GameConstants.SeedsLeftProp, crop.seedsToPlant.ToString()},
                            {GameConstants.YieldProp, crop.yieldAmount.ToString()}
                        }
                    };
                    itemsToUpdate.Add(itemToUpdate);
                }

                await _stellarClient.UpdateItems(itemsToUpdate);
                Debug.Log($"Inventory updated with new crop info");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update inventory: {e.Message}");
            }
        }

        public void UpdateCropInfo(long instanceId, int yieldAmount, int seedsAmount)
        {
            
        }

        public PlantInfo TryGetCropInfoByInstanceId(long instanceId)
        {
            return CropInstancesDictionary.GetValueOrDefault(instanceId);
        }
    }
}