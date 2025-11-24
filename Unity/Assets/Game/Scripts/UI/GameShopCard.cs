using System;
using Farm.Beam;
using Farm.Managers;
using Farm.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm.UI
{
    public class GameShopCard : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Buttons")]
        [SerializeField] private Button buySeedButton;
        [SerializeField] private Button sellOneYieldButton;
        [SerializeField] private Button sellAllYieldButton;
        [SerializeField] private Transform sellYieldButtonTransform;
        
        [Header("References")]
        [SerializeField] private Collider2D uiCollider;
        
        private bool _isSelectedByUI, _isCrop;
        private InventoryController _inventory;
        private const string SeedsText = "Seeds";
        private const string YieldText = "Yield";
        private const string CurrencySign = "$";
        
        public PlantInfo CurrentPlant { get; private set; }

        /// <summary>
        /// isCrop == false: This will be a seed buying card
        /// isCrop == true: This will be a yield selling card
        /// </summary>
        public void Init(InventoryController inventory, 
            PlantInfo plant, bool isCrop)
        {
            _inventory = inventory;
            CurrentPlant = plant;
            _isCrop = isCrop;
            iconImage.sprite = !_isCrop ? plant.cropData.seedsSprite : plant.cropData.cropIcon;
            SetTitle();
            SetAmount();
            SetButtonsStatus();
        }

        private void OnEnable()
        {
            CropManager.OnCropInfoUpdated += OnCropInfoUpdated;
        }

        private void OnDisable()
        {
            CropManager.OnCropInfoUpdated -= OnCropInfoUpdated;
        }

        private void SetAmount()
        {
            amountText.text = _isCrop ? $"x{CurrentPlant.yieldAmount}" 
                : $"{CurrencySign}{CurrentPlant.seedBuyPrice}";
            
            if(_isCrop && CurrentPlant.yieldAmount == 0) canvasGroup.alpha = 0.7f;
            else canvasGroup.alpha = 1f;
        }

        private void SetTitle()
        {
            nameText.text = _isCrop ? YieldText + " " + CurrentPlant.cropData.cropName 
                : SeedsText + " " +CurrentPlant.cropData.cropName;
        }

        private void SetButtonsStatus()
        {
            buySeedButton.gameObject.SetActive( !_isCrop );
            sellYieldButtonTransform.gameObject.SetActive( _isCrop );

            if (!_isCrop || CurrentPlant.yieldAmount < 1) return;
            sellOneYieldButton.interactable = true;
            sellAllYieldButton.interactable = true;
        }

        public void OnCropInfoUpdated(PlantInfo newInfo)
        {
            if (newInfo.cropData.cropType != CurrentPlant.cropData.cropType) return;
            CurrentPlant = newInfo;
            SetAmount();
            SetButtonsStatus();
            SetTitle();
        }

        #region Button_Methods

        public async void OnBuySeedButtonClicked()
        {
            try
            {
                _inventory.SetLoadingBlocker(true);
                await BeamManager.Instance.CommerceManager.UpdateCoinAmount(-CurrentPlant.seedBuyPrice);
                CropManager.Instance.AddSeeds(CurrentPlant.cropData.cropType, 1);
                _inventory.SetLoadingBlocker(false);
                AudioManager.Instance.PlaySfx(8);
            }
            catch (Exception e)
            {
                _inventory.SetLoadingBlocker(true, true, "You do not have enough coins to buy seeds.");
                Debug.LogError($"Failed to buy seeds for {CurrentPlant.cropData.cropName}: {e.Message}");
            }
        }

        public async void OnSellOneYieldButtonClicked()
        {
            try
            {
                _inventory.SetLoadingBlocker(true);
                await BeamManager.Instance.CommerceManager.UpdateCoinAmount(CurrentPlant.yieldSellPrice);
                CropManager.Instance.UseYield(CurrentPlant.cropData.cropType, 1);
                SetButtonsStatus();
                _inventory.SetLoadingBlocker(false);
                AudioManager.Instance.PlaySfx(9);
            }
            catch (Exception e)
            {
                _inventory.SetLoadingBlocker(true, true, $"Selling failed: {e.Message}");
                Debug.LogError($"Failed to sell yield for {CurrentPlant.cropData.cropName}: {e.Message}");
            }
        }

        public async void OnSellAllYieldButtonClicked()
        {
            try
            {
                _inventory.SetLoadingBlocker(true);
                var totalSellPrice = CurrentPlant.yieldAmount * CurrentPlant.yieldSellPrice;
                await BeamManager.Instance.CommerceManager.UpdateCoinAmount(totalSellPrice);
                CropManager.Instance.UseYield(CurrentPlant.cropData.cropType, CurrentPlant.yieldAmount);
                SetButtonsStatus();
                _inventory.SetLoadingBlocker(false);
                AudioManager.Instance.PlaySfx(9);
            }
            catch (Exception e)
            {
                _inventory.SetLoadingBlocker(true, true, $"Selling failed: {e.Message}");
                Debug.LogError($"Failed to sell all yield for {CurrentPlant.cropData.cropName}: {e.Message}");
            }
        }

        #endregion
    }
}