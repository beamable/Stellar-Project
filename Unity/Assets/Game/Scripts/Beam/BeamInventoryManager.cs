using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Beamable;
using Beamable.Common.Api.Inventory;
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

        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
        }

        // private void OnInvRefresh(InventoryView inventoryView)
        // {
        //     RefreshInventory(inventoryView).Forget();
        // }
        //
        // private async UniTask RefreshInventory(InventoryView inventoryView)
        // {
        //     Debug.Log($"Inventory Refreshing...");
        //     
        //     //check default crop
        //     //if (!await UpdateDefaultCropInfo(inventoryView)) return;
        //
        //     Debug.Log($"Inventory Refresh Done!");
        // }

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

            var defaultPlayerCrop = TryGetCropInfo(defaultCropInfo.instanceId);
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

        public PlantInfo TryGetCropInfo(long instanceId)
        {
            return CropInstancesDictionary.GetValueOrDefault(instanceId);
        }
    }
}