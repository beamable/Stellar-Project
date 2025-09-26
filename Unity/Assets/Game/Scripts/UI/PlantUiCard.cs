using System;
using Farm.Helpers;
using Farm.Managers;
using Farm.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Farm.UI
{
    public class PlantUiCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("UI Elements")] 
        [SerializeField] private Image iconImage;
        [SerializeField] private Image noStockImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Colors")] 
        [SerializeField] private Outline outline;
        [SerializeField] private string defaultColor = "#2A3D68";
        [SerializeField] private string selectedColor = "#F0E3B7";
        
        [SerializeField] private Collider2D uiCollider;
        
        private bool _isSelectedByUI, _isCrop;
        private InventoryController _inventory;
        public PlantInfo CurrentPlant { get; private set; }

        public void Init(bool isSelected, InventoryController inventory, 
            PlantInfo plant, int amount, bool isSeed = true)
        {
            _inventory = inventory;
            CurrentPlant = plant;
            nameText.text = plant.cropData.cropName;
            amountText.text = $"X{amount}";
            iconImage.sprite = isSeed ? plant.cropData.seedsSprite : plant.cropData.cropIcon;
            SetSelectedColor(isSelected);
        }

        private void OnEnable()
        {
            CropManager.OnCropInfoUpdated += OnCropInfoUpdated;
            UpdateAmountText(CurrentPlant);
        }

        private void OnDisable()
        {
            CropManager.OnCropInfoUpdated -= OnCropInfoUpdated;
        }

        private void OnCropInfoUpdated(PlantInfo newInfo)
        {
            if (newInfo.cropData.cropType != CurrentPlant.cropData.cropType) return;
            UpdateAmount(newInfo.seedsToPlant);
            CurrentPlant = newInfo;
            
            UpdateAmountText(newInfo);
        }

        private void UpdateAmountText(PlantInfo newInfo)
        {
            if(newInfo == null) return;
            var amount = _isCrop ? newInfo.yieldAmount : newInfo.seedsToPlant;
            amountText.text = $"X{amount}";

            if (!_isCrop && amount == 0)
            {
                canvasGroup.alpha = 0.7f;
                noStockImage.enabled = true;
            }
            else
            {
                noStockImage.enabled = false;
                canvasGroup.alpha = 1f;
            }
        }

        #region Pointer Events

        public void SetSelectedColor(bool isSelected)
        {
            outline.effectColor = isSelected ? GameUtilities.ParseHtmlColor(selectedColor) 
                : GameUtilities.ParseHtmlColor(defaultColor);
        }

        public void SetIsSelectedByUI(bool isSelected)
        {
            _isSelectedByUI = isSelected;
        }
        

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_isCrop) return;
            SetSelectedColor(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_isCrop) return;
            if(_isSelectedByUI) return;
            SetSelectedColor(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(_isCrop) return;
            SetIsSelectedByUI(true);
            _inventory.SetSelectedSeed(this);
        }

        #endregion

        public void SetIsCrop(bool isCrop)
        {
            _isCrop = isCrop;
            if(_isCrop) uiCollider.enabled = false;
            noStockImage.enabled = !_isCrop;
        }

        public void UpdateAmount(int amount)
        {
            amountText.text = $"X{amount}";
        }
    }
}