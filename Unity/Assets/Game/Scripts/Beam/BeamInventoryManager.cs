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
        [SerializeField] private CropItemRef testItemRef;

        private BeamContentManager _contentManager;
        
        public List<PlantInfo> PlayerCrops { get; private set; }
        public Dictionary<long, PlantInfo> CropInstancesDictionary { get; private set; } = new Dictionary<long, PlantInfo>();
        
        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            _contentManager = BeamManager.Instance.ContentManager;
            PlayerCrops = new List<PlantInfo>();
            await FetchInventory();

            IsReady = true;
        }

        [ContextMenu("Fetch Inventory" )]
        public async UniTask FetchInventory()
        {
            var inv = await GetCurrentInventoryView();
            await UpdateDefaultCropInfo(inv);
            
            if(inv.items.Count < 1) return;
            ProcessCropInstances(inv);
        }

        private void ProcessCropInstances(InventoryView inv)
        {
            foreach (var invItem in inv.items)
            {
                if (invItem.Key == defaultCropRef.Id) continue;
                
                
                var cropInfo = _contentManager.GetCropInfo(invItem.Key);
                var cropInstance = invItem.Value[0];
                cropInfo.instanceId = cropInstance.id;
                cropInfo.yieldAmount = cropInstance.properties.TryGetValue(GameConstants.YieldProp, out var yieldAmount)
                    ? int.Parse(yieldAmount)
                    : 0;
                cropInfo.seedsToPlant = cropInstance.properties.TryGetValue(GameConstants.SeedsLeftProp, out var seedsAmount)
                    ? int.Parse(seedsAmount)
                    : 0;
                if(CropInstancesDictionary.ContainsKey(invItem.Value[0].id))
                {
                    //Update crop info
                    CropInstancesDictionary[cropInstance.id] = cropInfo;
                    var index = PlayerCrops.FindIndex(x => x.instanceId == cropInstance.id);
                    PlayerCrops[index] = cropInfo;
                    continue;
                }
                CropInstancesDictionary.TryAdd(cropInstance.id, cropInfo);
                PlayerCrops.Add(cropInfo);
            }
        }

        [ContextMenu("Add Crop")]
        public async UniTask AddCrop() //TODO: update with param content id
        {
            try
            {
                if (await IsItemOwned(testItemRef.Id))
                {
                    Debug.LogWarning($"Item {testItemRef.Id} already owned");
                    return;
                }
                var cropInfo = _contentManager.GetCropInfo(testItemRef.Id);
                var properties = new Dictionary<string, string>
                {
                    { GameConstants.SeedsLeftProp, cropInfo.seedsToPlant.ToString() },
                    { GameConstants.YieldProp, cropInfo.yieldAmount.ToString() }
                };
                await _stellarClient.AddItem(testItemRef.Id, properties);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to add crop: {e.Message}");
            }
        }
        
        public PlantInfo TryGetCropInfoByInstanceId(long instanceId)
        {
            return CropInstancesDictionary.GetValueOrDefault(instanceId);
        }

        private async UniTask<InventoryView> GetCurrentInventoryView()
        {
            await _beamContext.Inventory.Refresh();
            var inv = await _beamContext.Api.InventoryService.GetCurrent();
            return inv;
        }

        private async UniTask<bool> IsItemOwned(string contentId)
        {
            var inv = await GetCurrentInventoryView();
            return inv.items.ContainsKey(contentId);
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

        public bool AlreadyOwned(string contentId)
        {
            return PlayerCrops.Any(crop => crop.contentId == contentId && crop.IsOwned);
        }
        
    }
}