using System.Collections;
using System.Collections.Generic;
using Beamable.Common.Shop;
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
        [SerializeField] private MainMenuShopCard shopCard;
        
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
        private List<MainMenuShopCard> _shopCards = new List<MainMenuShopCard>();
        private List<PlantInfo> PlayerCrops => BeamManager.Instance.InventoryManager.PlayerCrops;
        private List<ListingContent> StoreListings => BeamManager.Instance.CommerceManager.Listings;

        public bool IsPurchasing { get; private set; }
        #endregion

        #region Unity_Methods

        private async void Start()
        {
            await UniTask.WaitUntil(()=> BeamManager.Instance.InventoryManager.IsReady);
            UpdatePlayerCurrency(PlayerCurrency);
            SetLoadingBlocker(false);
        }

        private void OnEnable()
        {
            BeamCommerceManager.OnCoinCountUpdated += UpdatePlayerCurrency;
            BeamInventoryManager.OnInventoryUpdated += PopulateShop;
        }

        private void OnDisable()
        {
            BeamCommerceManager.OnCoinCountUpdated -= UpdatePlayerCurrency;
            BeamInventoryManager.OnInventoryUpdated -= PopulateShop;
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
            IsPurchasing = false;
            SetLoadingBlocker(false);
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
                if (autoDeactivate)
                {
                    loadingPanel.SetActive(false);
                    IsPurchasing = false;
                }
            }
            
        }

        private void PopulateInventoryTab()
        {
            foreach (var plantInfo in PlayerCrops)
            {
                var card = Instantiate(plantUiCard, inventoryContainer);
                card.Init(false, false, null, plantInfo, plantInfo.yieldAmount);
                card.transform.SetParent(inventoryContainer);
                card.SetSelectedImage(false);
                card.IsSelectable(false);
                card.ForceRemoveStockImage(true);
                _inventoryCards.Add(card);
            }
        }

        //TODO: Populate later from inventory
        private void PopulateShopTab()
        {
            foreach (var listing in StoreListings)
            {
                var id = listing.offer.obtainItems[0].contentId;
                var info = BeamManager.Instance.ContentManager.GetCropInfo(id.Id);
                var card = Instantiate(shopCard, shopContainer);
                
                var isOwned = BeamManager.Instance.InventoryManager.AlreadyOwned(id);
                card.Init(this, info, listing, isOwned);
                card.transform.SetParent(shopContainer);
                _shopCards.Add(shopCard);
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

        public void SetIsPurchasing(bool purchasing)
        {
            this.IsPurchasing = purchasing;
        }
    }
}