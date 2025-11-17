using System;
using System.Threading;
using Beamable.Common.Inventory;
using Cysharp.Threading.Tasks;
using StellarFederationCommon.FederationContent;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamCommerceManager : BeamManagerBase
    {
        //TODO: Change to a real currency reference
        [SerializeField] private CoinCurrencyRef coinRef;
        [SerializeField] private CurrencyRef testCoinRef;

        public int CurrentCoinCount { get; private set; }

        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            await GetServerCoinCount();
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
    }
}