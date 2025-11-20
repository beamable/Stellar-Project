using System;
using System.Collections.Generic;
using Farm.Beam;
using Farm.Managers;
using Farm.UI;
using TMPro;
using UnityEngine;

namespace Farm.Player
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI inventoryTitle;
        
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
        
        private List <PlantUiCard> _seedsCards = new List<PlantUiCard>();

        private UiManager _uiManager;
        private ToolsBarManager _toolsBarPanel;
        private PlantUiCard _selectedSeedCard = null;
        private List<PlantInfo> CropInfos => BeamManager.Instance.InventoryManager.PlayerCrops;
        
        private const string SeedsTab = "Player Seeds";
        private const string YieldTab = "Yield Shop";
        private const string ShopTab = "Seeds Shop";

        public void InitAwake(UiManager uiManager, ToolsBarManager toolsBarManager)
        {
            _uiManager = uiManager;
            _toolsBarPanel = toolsBarManager;
        }
        
        public void OpenInventory()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            _uiManager.RaiseOpenUi(gameObject.activeSelf);

            if (!gameObject.activeSelf) return;
            OnSelectSeedTab();
            PopulateSeedsInventory(CropInfos);
            PopulateShopSeeds(CropInfos);
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

        public void PopulateSeedsInventory(List<PlantInfo> cropInfos)
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
        
        private void PopulateShopSeeds(List<PlantInfo> cropInfos)
        {
            foreach (var crop in cropInfos)
            {
                var seedCard = Instantiate(shopCard, shopContainer);
                seedCard.Init(this, crop, 0, false);
                seedCard.transform.SetParent(shopContainer);
                
            }
        }

        #region Button Events

        public void OnSelectSeedTab()
        {
            inventoryTitle.text = SeedsTab;
            seedsCanvasGroup.alpha = 1;
            yieldCanvasGroup.alpha = 0;
            shopCanvasGroup.alpha = 0;
        }

        public void OnSelectYieldTab()
        {
            inventoryTitle.text = YieldTab;
            seedsCanvasGroup.alpha = 0;
            yieldCanvasGroup.alpha = 1;
            shopCanvasGroup.alpha = 0;
        }

        public void OnSelectShopTab()
        {
            inventoryTitle.text = ShopTab;
            seedsCanvasGroup.alpha = 0;
            yieldCanvasGroup.alpha = 0;
            shopCanvasGroup.alpha = 1;
        }
        
        #endregion
        
    }
}