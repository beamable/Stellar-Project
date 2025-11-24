using System;
using System.Collections;
using System.Collections.Generic;
using Farm.Beam;
using Farm.Managers;
using Farm.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Player
{
    public class InventoryController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private TextMeshProUGUI inventoryTitle;
        [SerializeField] private Button seedButton;
        
        [Header("Loading")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private TextMeshProUGUI loadingText;
        
        [Header("Currency")] 
        [SerializeField] private TextMeshProUGUI currencyValue;
        
        [Header("Cards Prefab")]
        [SerializeField] private PlantUiCard plantUiCard;
        [SerializeField] private GameShopCard shopCard;
        
        [Header("Containers")]
        [SerializeField] private Transform seedsContainer;
        [SerializeField] private Transform yieldContainer;
        [SerializeField] private Transform shopContainer;
        
        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup seedsCanvasGroup;
        [SerializeField] private CanvasGroup yieldCanvasGroup;
        [SerializeField] private CanvasGroup shopCanvasGroup;
        
        private int PlayerCurrency => BeamManager.Instance.CommerceManager.CurrentCoinCount;
        private List <PlantUiCard> _seedsCards = new List<PlantUiCard>();
        private List <GameShopCard> _seedShopCards = new List<GameShopCard>();
        private List <GameShopCard> _yieldShopCards = new List<GameShopCard>();

        private UiManager _uiManager;
        private ToolsBarManager _toolsBarPanel;
        private PlantUiCard _selectedSeedCard = null;
        private List<PlantInfo> CropInfos => BeamManager.Instance.InventoryManager.PlayerCrops;
        
        private const string SeedsTab = "Player Seeds";
        private const string YieldTab = "Yield Shop";
        private const string ShopTab = "Seeds Shop";

        #endregion

        #region Unity Methods

        public void InitAwake(UiManager uiManager, ToolsBarManager toolsBarManager)
        {
            _uiManager = uiManager;
            _toolsBarPanel = toolsBarManager;
            UpdatePlayerCurrency(PlayerCurrency);
            SetLoadingBlocker(false);
        }

        private void OnEnable()
        {
            BeamCommerceManager.OnCoinCountUpdated += UpdatePlayerCurrency;
        }

        private void OnDisable()
        {
            BeamCommerceManager.OnCoinCountUpdated -= UpdatePlayerCurrency;
        }
        
        public void PopulateInventory(List<PlantInfo> cropInfos)
        {
            PopulateSeedsInventory(cropInfos);
            PopulateShopSeeds(cropInfos);
            PopulateYieldShop(cropInfos);
        }
        
        private void PopulateSeedsInventory(List<PlantInfo> cropInfos)
        {
            for (var i = 0; i < cropInfos.Count; i++)
            {
                var card = Instantiate(plantUiCard, seedsContainer);
                card.Init(false, false,this, cropInfos[i], cropInfos[i].seedsToPlant);
                card.transform.SetParent(seedsContainer);

                card.SetSelectedImage(i == 0);
                if (i == 0)
                {
                    SetSelectedSeed(card);
                }
                _seedsCards.Add(card);
            }
        }

        private void PopulateYieldShop(List<PlantInfo> cropInfos)
        {
            foreach (var crop in cropInfos)
            {
                var seedCard = Instantiate(shopCard, yieldContainer);
                seedCard.Init(this, crop, true);
                seedCard.transform.SetParent(yieldContainer);
                _yieldShopCards.Add(seedCard);
            }
        }
        
        private void PopulateShopSeeds(List<PlantInfo> cropInfos)
        {
            foreach (var crop in cropInfos)
            {
                var seedCard = Instantiate(shopCard, shopContainer);
                seedCard.Init(this, crop, false);
                seedCard.transform.SetParent(shopContainer);
                _seedShopCards.Add(seedCard);
            }
        }

        #endregion

        private void UpdatePlayerCurrency(int obj)
        {
            currencyValue.text = PlayerCurrency.ToString();
        }

        public void OpenInventory()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            _uiManager.RaiseOpenUi(gameObject.activeSelf);

            if (!gameObject.activeSelf) return;
            seedButton.Select();
            OnSelectSeedTab();
            UpdateAvailableSeeds();
            UpdateYieldShop();
        }

        private void UpdateAvailableSeeds()
        {
            foreach (var seedsCard in _seedsCards)
            {
                var seedInfo = CropManager.Instance.GetCropInfo(seedsCard.CurrentPlant.cropData.cropType);
                seedsCard.UpdateAmount(seedInfo.seedsToPlant);
            }
        }

        private void UpdateYieldShop()
        {
            foreach (var yieldCard in _yieldShopCards)
            {
                var yieldInfo = CropManager.Instance.GetCropInfo(yieldCard.CurrentPlant.cropData.cropType);
                yieldCard.OnCropInfoUpdated(yieldInfo);
            }
        }

        public void ForceCloseInventory()
        {
            gameObject.SetActive(false);
            _uiManager.RaiseOpenUi(false);
        }

        public void SetSelectedSeed(PlantUiCard card)
        {
            if (_selectedSeedCard != null)
            {
                _selectedSeedCard.SetSelectedImage(false);
                _selectedSeedCard.SetIsSelectedByUI(false);
            }
            _selectedSeedCard = card;
            _selectedSeedCard.SetSelectedImage(true);
            _selectedSeedCard.SetIsSelectedByUI(true);
            _toolsBarPanel.SetSeedSprite(card.CurrentPlant.cropData.seedsSprite);
            UiManager.Instance.RaiseSelectSeed(card.CurrentPlant);
        }

        private void SetCanvasGroup(CanvasGroup canvasGroup, bool isActive)
        {
            canvasGroup.alpha = isActive ? 1 : 0;
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }

        #region Button Events

        public void OnSelectSeedTab()
        {
            inventoryTitle.text = SeedsTab;
            SetCanvasGroup(seedsCanvasGroup, true);
            SetCanvasGroup(yieldCanvasGroup, false);
            SetCanvasGroup(shopCanvasGroup, false);
        }

        public void OnSelectYieldTab()
        {
            inventoryTitle.text = YieldTab;
            SetCanvasGroup(seedsCanvasGroup, false);
            SetCanvasGroup(yieldCanvasGroup, true);
            SetCanvasGroup(shopCanvasGroup, false);
        }

        public void OnSelectShopTab()
        {
            inventoryTitle.text = ShopTab;
            SetCanvasGroup(seedsCanvasGroup, false);
            SetCanvasGroup(yieldCanvasGroup, false);
            SetCanvasGroup(shopCanvasGroup, true);
        }

        public void SetLoadingBlocker(bool isLoading, bool autoDeactivate = false, string text = "Loading...")
        {
            loadingPanel.SetActive(isLoading);
            loadingText.text = text;
            
            if (isLoading) StartCoroutine(WaitAndDeactivate());
            return;
            
            IEnumerator WaitAndDeactivate()
            {
                yield return new WaitForSeconds(3);
                if (autoDeactivate) gameObject.SetActive(false);
            }
            
        }
        
        #endregion
        
    }
}