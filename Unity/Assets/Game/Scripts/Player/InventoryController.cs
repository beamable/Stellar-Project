using System;
using System.Collections.Generic;
using Farm.Managers;
using Farm.UI;
using UnityEngine;

namespace Farm.Player
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private PlantUiCard plantUiCard;
        [SerializeField] private Transform seedsContainer;
        
        private List <PlantUiCard> _seedsCards = new List<PlantUiCard>();

        private UiManager _uiManager;
        private ToolsBarManager _toolsBarPanel;
        private PlantUiCard _selectedSeedCard = null;

        public void InitAwake(UiManager uiManager, ToolsBarManager toolsBarManager)
        {
            _uiManager = uiManager;
            _toolsBarPanel = toolsBarManager;
        }
        
        public void OpenInventory()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            _uiManager.RaiseOpenUi(gameObject.activeSelf);
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
                _selectedSeedCard.SetSelectedColor(false);
                _selectedSeedCard.SetIsSelectedByUI(false);
            }
            _selectedSeedCard = card;
            _selectedSeedCard.SetSelectedColor(true);
            _selectedSeedCard.SetIsSelectedByUI(true);
            _toolsBarPanel.SetSeedSprite(card.CurrentPlant.cropData.seedsSprite);
            UiManager.Instance.RaiseSelectSeed(card.CurrentPlant);
        }

        public void PopulateSeeds(List<PlantInfo> cropInfos)
        {
            for (var i = 0; i < cropInfos.Count; i++)
            {
                var card = Instantiate(plantUiCard, seedsContainer);
                card.Init(false, this, cropInfos[i], cropInfos[i].seedsToPlant);
                card.transform.SetParent(seedsContainer);

                card.SetSelectedColor(i == 0);
                if (i == 0)
                {
                    SetSelectedSeed(card);
                }
                card.SetIsCrop(false);
                _seedsCards.Add(card);
            }
        }
        
    }
}