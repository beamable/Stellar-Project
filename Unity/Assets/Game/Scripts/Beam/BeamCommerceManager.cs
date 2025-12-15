using System;
using System.Collections.Generic;
using System.Threading;
using Beamable;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Inventory;
using Beamable.Common.Shop;
using Cysharp.Threading.Tasks;
using StellarFederationCommon.FederationContent;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamCommerceManager : BeamManagerBase
    {
        //TODO: Change to a real currency reference
        [Header("Currency")]
        [SerializeField] private CoinCurrencyRef coinRef;
        [SerializeField] private CurrencyRef testCoinRef;
        [SerializeField] private bool useTestCoin = false;

        [Header("Store")] 
        [SerializeField] private StoreRef storeRefNf;

        private StoreContent _storeContent;
        public int CurrentCoinCount { get; private set; }
        public List<ListingContent> Listings { get; private set; }
        
        //Actions
        public static Action<int> OnCoinCountUpdated;

        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            await ResolveShopListings();
            _beamContext.Api.InventoryService.Subscribe(GetCurrencyId(), OnCurrencyUpdated);
        }


        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
        }

        private void OnCurrencyUpdated(InventoryView view)
        {
            if (!BeamManager.Instance.InventoryManager.IsDirty(view)) return; 
            var currency = view.currencies;
            currency.TryGetValue(GetCurrencyId(), out var value);
            CurrentCoinCount = (int)value;
            OnCoinCountUpdated?.Invoke(CurrentCoinCount);
        }
        
        [ContextMenu("Add Coins")]
        public async void AddCurrency()
        {
            await UpdateCoinAmount(50);
        }
        
        public async UniTask UpdateCoinAmount(int amount)
        {
            try
            {
                await _stellarClient.UpdateCurrency(GetCurrencyId(), amount);
                Debug.Log($"Updated Currency to {amount}. Current Balance:{CurrentCoinCount}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Updating Currency Failed: {e.Message}");
            }            
        }
        
        private async UniTask ResolveShopListings()
        {
            _storeContent = await storeRefNf.Resolve();
            Listings = new List<ListingContent>();
            foreach (var listing in _storeContent.listings)
            {
                var resolved = await listing.Resolve();
                Listings.Add(resolved);
            }
        }
        
        public async UniTask PurchaseListing(ListingContent listing)
        {
            try
            {
                await _beamContext.Api.CommerceService.Purchase(storeRefNf.Id, listing.Id);
                await _beamContext.Inventory.Refresh();
                await BeamManager.Instance.InventoryManager.ForceIsRefreshing();
                await UniTask.WaitUntil(()=> !BeamManager.Instance.InventoryManager.IsRefreshing);
                var crop = BeamManager.Instance.ContentManager.GetCropInfo(listing.offer.obtainItems[0].contentId);
                if(crop == null) return;
                await BeamManager.Instance.InventoryManager.UpdateSpecificCropInfo(listing.offer.obtainItems[0].contentId, crop.yieldAmount, crop.cropData.startingSeedsAmount);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to purchase listing: {e.Message}");
            }
        }

        private string GetCurrencyId()
        {
            return useTestCoin ? testCoinRef.Id : coinRef.Id;
        }
    }
}