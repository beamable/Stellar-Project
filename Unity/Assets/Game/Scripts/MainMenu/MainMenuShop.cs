using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Farm.Beam;
using Farm.Helpers;
using Farm.Managers;
using Farm.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Game.Scripts.MainMenu
{
    public class MainMenuShop : MonoBehaviour
    {
        #region SerializedFields

        [SerializeField] private TextMeshProUGUI shopTitle;
        [SerializeField] private Button inventoryTab;
        
        [Header("Loading")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private TextMeshProUGUI loadingText;
        
        [Header("Currency")]
        [SerializeField] private TextMeshProUGUI currencyValue;
        
        [Header("Cards Prefab")]
        [SerializeField] private PlantUiCard plantUiCard;
        [SerializeField] private GameShopCard shopCard;
        
        [Header("Containers")]
        [SerializeField] private Transform inventoryContainer;
        [SerializeField] private Transform shopContainer;
        
        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup inventoryCanvasGroup;
        [SerializeField] private CanvasGroup shopCanvasGroup;

        #endregion

        #region Variables
        
        private const string InventoryTabText = "Inventory";
        private const string ShopTabText = "Crop Shop";

        private int PlayerCurrency => BeamManager.Instance.CommerceManager.CurrentCoinCount;
        private List<PlantUiCard> _inventoryCards = new List<PlantUiCard>();
        private List<GameShopCard> _shopCards = new List<GameShopCard>();
        private List<PlantInfo> CropInfos => BeamManager.Instance.InventoryManager.PlayerCrops;
        

        #endregion

        #region Unity_Methods

        private async void Start()
        {
            await UniTask.WaitUntil(()=> BeamManager.Instance.InventoryManager.IsReady);
            UpdatePlayerCurrency(PlayerCurrency);
            SetLoadingBlocker(false);
            Debug.LogWarning($"Main Menu Shop Ready");
        }

        private void OnEnable()
        {
            BeamCommerceManager.OnCoinCountUpdated += UpdatePlayerCurrency;
        }

        private void OnDisable()
        {
            BeamCommerceManager.OnCoinCountUpdated -= UpdatePlayerCurrency;
        }

        #endregion

        public void OpenShop()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            
            if(!gameObject.activeSelf) return;
            inventoryTab.Select();
            UpdatePlayerCurrency(PlayerCurrency);
            SetLoadingBlocker(false);
            OnSelectedInventoryTab();
            PopulateShop();
        }

        private void PopulateShop()
        {
            CleanUpContainers();

            PopulateInventoryTab();
            PopulateShopTab();
        }

        private void CleanUpContainers()
        {
            //clean up inventory container first
            foreach (Transform child in inventoryContainer) Destroy(child.gameObject);
            _inventoryCards.Clear();
            //clean up shop container first
            foreach (Transform child in shopContainer) Destroy(child.gameObject);
            _shopCards.Clear();
        }

        private void UpdatePlayerCurrency(int currency)
        {
            currencyValue.text = PlayerCurrency.ToString();
        }
        
        private void SetLoadingBlocker(bool isLoading, bool autoDeactivate = false, string text = "Loading...")
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

        private void PopulateInventoryTab()
        {
            foreach (var plantInfo in CropInfos)
            {
                var card = Instantiate(plantUiCard, inventoryContainer);
                card.Init(false, false, null, plantInfo, plantInfo.seedsToPlant);
                card.transform.SetParent(inventoryContainer);
                card.SetSelectedImage(false);
                card.IsSelectable(false);
                card.SetStockImageStatus(false);
                _inventoryCards.Add(card);
            }
        }

        //TODO: Populate later from inventory
        private void PopulateShopTab()
        {
            // foreach (var plantInfo in CropInfos)
            // {
            //     
            // }
        }
        
        private void UpdateAvailableSeeds()
        {
            foreach (var seedsCard in _inventoryCards)
            {
                var seedInfo = CropManager.Instance.GetCropInfo(seedsCard.CurrentPlant.cropData.cropType);
                seedsCard.UpdateAmount(seedInfo.seedsToPlant);
            }
        }
        
        #region Tabs_Events

        public void OnSelectedInventoryTab()
        {
            shopTitle.text = InventoryTabText;
            GameUtilities.SetCanvasGroup(inventoryCanvasGroup, true);
            GameUtilities.SetCanvasGroup(shopCanvasGroup, false);
        }
        
        public void OnSelectedShopTab()
        {
            shopTitle.text = ShopTabText;
            GameUtilities.SetCanvasGroup(inventoryCanvasGroup, false);
            GameUtilities.SetCanvasGroup(shopCanvasGroup, true);
        }

        #endregion
    }
}