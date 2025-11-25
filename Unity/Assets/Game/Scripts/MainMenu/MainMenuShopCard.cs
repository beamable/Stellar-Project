using System;
using Beamable.Common.Shop;
using Farm.Beam;
using Farm.Managers;
using Farm.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Game.Scripts.MainMenu
{
    public class MainMenuShopCard : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Buttons")]
        [SerializeField] private Button purchaseButton;
        [SerializeField] private GameObject ownedObject;

        private bool _isOwned;
        private const string CurrencySign = "$";
        private MainMenuShop _mainMenuShop;
        private ListingContent _listing;
        public PlantInfo CurrentPlant { get; private set; }

        /// <summary>
        /// isCrop == false: This will be a seed buying card
        /// isCrop == true: This will be a yield selling card
        /// </summary>
        public void Init(MainMenuShop shop, PlantInfo plant, ListingContent listing, bool isOwned)
        {
            _mainMenuShop = shop;
            _listing = listing;
            CurrentPlant = plant;
            iconImage.sprite = plant.cropData.cropIcon;
            _isOwned = isOwned;
            SetTitle();
            SetAmount(listing.price.amount);
            SetButtonsStatus();
        }

        private void SetAmount(int price)
        {
            priceText.text = price == 0 ? "Free" : $"{CurrencySign}{price}";
        }

        private void SetTitle()
        {
            nameText.text = CurrentPlant.cropData.cropName;
        }

        private void SetButtonsStatus()
        {
            purchaseButton.gameObject.SetActive( !_isOwned );
            ownedObject.SetActive( _isOwned );
        }

        #region Button_Methods

        public async void OnPurchaseListingClicked()
        {
            if (_listing.price.amount > BeamManager.Instance.CommerceManager.CurrentCoinCount)
            {
                _mainMenuShop.SetLoadingBlocker(true, true, "You do not have enough coins to purchase crop.");
                return;
            }
            
            try
            {
                _mainMenuShop.SetLoadingBlocker(true);
                await BeamManager.Instance.CommerceManager.PurchaseListing(_listing);
                _mainMenuShop.SetLoadingBlocker(false);
                AudioManager.Instance.PlaySfx(8);
                _isOwned = true;
                SetButtonsStatus();
            }
            catch (Exception e)
            {
                _mainMenuShop.SetLoadingBlocker(true, true, "You do not have enough coins to purchase crop.");
                Debug.LogError($"Failed to purchase crop for {CurrentPlant.cropData.cropName}: {e.Message}");
            }
        }

        #endregion
    }
}