using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Player;
using Cysharp.Threading.Tasks;
using Farm.Helpers;
using Farm.Managers;
using StellarFederationCommon.FederationContent;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamInventoryManager : BeamManagerBase
    {
        #region Variables

        [Header("Stellar Mint")]
        [SerializeField] private int stellarMintTime;
        
        [Header("Crop Refs")]
        [SerializeField] private CropItemRef defaultCropRef;
        [SerializeField] private CropItemRef testItemRef;
        
        private int _lastInvHash;
        private bool _addingDefaultCrop = false;
        private bool _isReseting = false;
        private const string AnonymousAlias = "Anonymous";
        private BeamContentManager _contentManager;
        
        public bool IsRefreshing { get; private set; }
        public List<PlantInfo> PlayerCrops { get; private set; }
        public Dictionary<long, PlantInfo> CropInstancesDictionary { get; private set; } = new Dictionary<long, PlantInfo>();
        
        public static event Action OnInventoryUpdated;

        #endregion

        #region Init

        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            _contentManager = BeamManager.Instance.ContentManager;
            PlayerCrops = new List<PlantInfo>();
            CropInstancesDictionary = new Dictionary<long, PlantInfo>();
            IsReady = true;
            await _beamContext.Inventory.Refresh();
            if (!_isReseting) return;
            
            await FetchInventory(await GetCurrentInventoryView());
            _isReseting = false;
        }

        private void OnEnable()
        {
            OnEnableInit().Forget();
        }

        private async UniTask OnEnableInit()
        {
            await UniTask.WaitUntil(() => BeamManager.Instance.AccountManager.IsReady);
            await UniTask.WaitUntil(IsAliasSet);
            await UniTask.WaitUntil(()=> this.IsReady);
            _beamContext.Api.InventoryService.Subscribe(OnInvRefresh);
        }

        private bool IsAliasSet()
        {
            return !string.Equals(AnonymousAlias,BeamManager.Instance.AccountManager.CurrentAccount.Alias);
        }

        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
            PlayerCrops.Clear();
            CropInstancesDictionary.Clear();
            _lastInvHash = 0;
            IsRefreshing = false;
            _addingDefaultCrop = false;
            _isReseting = true;
        }

        #endregion

        private void OnInvRefresh(InventoryView view)
        {
            if (!IsReady || IsRefreshing) return;
            if (!IsDirty(view) && PlayerCrops.Count > 0) return; 
            IsRefreshing = true;
            FetchInventory(view).ContinueWith(() =>
            {
                IsRefreshing = false;
            });
        }

        [ContextMenu("Fetch Inventory" )]
        private async UniTask FetchInventory(InventoryView inv)
        {
            await UpdateDefaultCropInfo(inv);
            
            if(inv.items.Count < 1) return;
            ProcessCropInstances(inv);
            await UniTask.Yield();
            OnInventoryUpdated?.Invoke();
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
                if(_addingDefaultCrop) return false;
                _addingDefaultCrop = true;
                var properties = new Dictionary<string, string>
                {
                    { GameConstants.SeedsLeftProp, defaultCropInfo.seedsToPlant.ToString() },
                    { GameConstants.YieldProp, defaultCropInfo.yieldAmount.ToString() }
                };
                await _stellarClient.AddItem(defaultCropRef.Id, properties);
                return false;
            }

            _addingDefaultCrop = false;
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
            else if (PlayerCrops.Count < 1)
            {
                CropInstancesDictionary[cropInstance.id] = defaultCropInfo;
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
                var itemsToUpdate = GetCropUpdateRequests();

                await _stellarClient.UpdateItems(itemsToUpdate);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update inventory: {e.Message}");
            }
        }

        public List<CropUpdateRequest> GetCropUpdateRequests()
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

            return itemsToUpdate;
        }

        public async UniTask UpdateSpecificCropInfo(string contentId, int yieldAmount, int seedsLeft)
        {
            var crop = PlayerCrops.FirstOrDefault(x => x.contentId == contentId);
            if(crop == null) return;
            try
            {
                var itemsToUpdate = new List<CropUpdateRequest>();

                var itemToUpdate = new CropUpdateRequest()
                {
                    ContentId = crop.contentId,
                    InstanceId = crop.instanceId,
                    Properties = new Dictionary<string, string>()
                    {
                        { GameConstants.SeedsLeftProp, seedsLeft.ToString() },
                        { GameConstants.YieldProp, yieldAmount.ToString() }
                    }
                };
                itemsToUpdate.Add(itemToUpdate);
                await _stellarClient.UpdateItems(itemsToUpdate);
                IsRefreshing = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update crop {crop.contentId}: {e.Message}");
            }
        }
        
        public bool AlreadyOwned(string contentId)
        {
            return PlayerCrops.Any(crop => crop.contentId == contentId && crop.IsOwned);
        }

        public async UniTask ForcedStellarMintWaitingTime()
        {
            await UniTask.WaitForSeconds(stellarMintTime);
        }

        public async UniTask ForceIsRefreshing()
        {
            IsRefreshing = true;
            await UniTask.Yield();
        }
        
        /// <summary>
        /// Checks if the inventory has changed since the last time it was checked
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public bool IsDirty(InventoryView view)
        {
            unchecked
            {
                int hash = 17;

                foreach (var kvp in view.items)
                {
                    hash = hash * 23 + kvp.Key.GetHashCode();
                    hash = hash * 23 + kvp.Value.Count;
                    foreach (var item in kvp.Value)
                    {
                        hash = hash * 23 + item.id.GetHashCode();
                        foreach (var prop in item.properties)
                        {
                            hash = hash * 23 + prop.Key.GetHashCode();
                            hash = hash * 23 + prop.Value.GetHashCode();
                        }
                    }
                }

                foreach (var kvp in view.currencies)
                {
                    hash = hash * 23 + kvp.Key.GetHashCode();
                    hash = hash * 23 + kvp.Value.GetHashCode();
                }

                if (hash == _lastInvHash) return false;
                _lastInvHash = hash;
                return true;
            }
        }
    }
}