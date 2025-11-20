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
        
        [Header("References")]
        [SerializeField] private Collider2D uiCollider;
        
        private bool _isSelectedByUI, _isCrop, _isSeed;
        private InventoryController _inventory;
        private const string SeedsText = "Seeds";
        private const string YieldText = "Yield";
        private const string DollarText = "$";
        
        public PlantInfo CurrentPlant { get; private set; }

        /// <summary>
        /// isCrop == false: This will be a seed buying card
        /// isCrop == true: This will be a yield selling card
        /// </summary>
        public void Init(InventoryController inventory, 
            PlantInfo plant, int amount, bool isCrop)
        {
            _inventory = inventory;
            CurrentPlant = plant;
            amountText.text = $"X{amount}";
            _isCrop = isCrop;
            iconImage.sprite = !_isCrop ? plant.cropData.seedsSprite : plant.cropData.cropIcon;
            SetTitle();
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

        private void SetAmount()
        {
            if (_isCrop)
            {
                
            }
            else
            {
                
            }
        }

        private void SetTitle()
        {
            nameText.text = _isCrop ? YieldText + CurrentPlant.cropData.cropName 
                : SeedsText + CurrentPlant.cropData.cropName;
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
            }
            else
            {
                canvasGroup.alpha = 1f;
            }
        }

        public void UpdateAmount(int amount)
        {
            amountText.text = $"X{amount}";
        }
    }
}