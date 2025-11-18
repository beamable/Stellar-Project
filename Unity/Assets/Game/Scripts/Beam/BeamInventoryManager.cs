using System.Collections.Generic;
using System.Threading;
using Beamable;
using Beamable.Common.Api.Inventory;
using Cysharp.Threading.Tasks;
using StellarFederationCommon.FederationContent;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamInventoryManager : BeamManagerBase
    {
        [SerializeField] private CropItemRef defaultCropRef;

        private BeamContentManager _contentManager;
        
        public List<PlantInfo> PlayerCrops { get; private set; }
        
        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            _contentManager = BeamManager.Instance.ContentManager;
            PlayerCrops = new List<PlantInfo>();
            
            _beamContext.Api.InventoryService.Subscribe(OnInvRefresh);
        }

        

        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
        }

        private void OnInvRefresh(InventoryView inventoryView)
        {
            RefreshInventory(inventoryView).Forget();
        }
        
        private async UniTask RefreshInventory(InventoryView inventoryView)
        {
            Debug.Log($"Inventory Refreshing...");
            
            //check default crop
            if (inventoryView.items.Count < 1 || !inventoryView.items.ContainsKey(defaultCropRef.Id))
            {
                await _stellarClient.AddItem(defaultCropRef.Id, null);
            }
            
            PlayerCrops.Add(_contentManager.GetCropInfo(defaultCropRef.Id));
            
            
            Debug.Log($"Inventory Refresh Done!");
        }
    }
}