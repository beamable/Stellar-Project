using System;
using System.Collections.Generic;
using System.Threading;
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
            await GetServerCoinCount();

            await ResolveShopListings();
            IsReady = true;
        }

        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
        }

        /// <summary>
        /// Get the current coin count from the server and sync it to local storage
        /// </summary>
        /// <returns></returns>
        public async UniTask<int> GetServerCoinCount()
        {
            var serverCoin = await _beamContext.Inventory.LoadCurrencies(testCoinRef.Id);
            await serverCoin.OnReady;
            CurrentCoinCount = (int)serverCoin.GetCurrency(testCoinRef.Id).Amount;
            OnCoinCountUpdated?.Invoke(CurrentCoinCount);
            return CurrentCoinCount;
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
                await _stellarClient.UpdateCurrency(testCoinRef.Id, amount);
                CurrentCoinCount = await GetServerCoinCount();
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
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to purchase listing: {e.Message}");
            }
        }
    }
}