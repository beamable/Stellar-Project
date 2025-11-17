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
            GetServerCoinCount();
        }
        
        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
        }

        /// <summary>
        /// Get the current coin count from the server and sync it to local storage
        /// </summary>
        /// <returns></returns>
        public int GetServerCoinCount()
        {
            var coins = _beamContext.Inventory.GetCurrency(testCoinRef.Id);
            CurrentCoinCount = (int)coins.Amount;
            return CurrentCoinCount;
        }

        [ContextMenu("Add Coins")]
        public async void AddCurrency()
        {
            await UpdateCoinAmount(CurrentCoinCount + 50);
        }
        
        public async UniTask UpdateCoinAmount(int amount)
        {
            try
            {
                CurrentCoinCount = amount;
                await _stellarClient.UpdateCurrency(testCoinRef.Id, amount);
                Debug.Log($"Updated Currency to {amount}. Current Balance:{CurrentCoinCount}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Updating Currency Failed: {e.Message}");
            }            
        }
    }
}