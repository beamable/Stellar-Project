using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Farm.Beam;
using Farm.Input;
using Farm.Managers;
using Farm.Player;
using Farm.Time;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.UI
{
    public class UiManager : MonoSingleton<UiManager>
    {
        #region Variables

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI dayValueText;
        [SerializeField] private TextMeshProUGUI timeValueText;
        [SerializeField] private TextMeshProUGUI goToSleepText;
        [SerializeField] private TextMeshProUGUI awakeText;

        [Header("Panels")]
        [SerializeField] private SoPlayerInput playerInput;
        [SerializeField] private InventoryController inventoryPanel;
        [SerializeField] private CropsUiController cropUiController;
        [SerializeField] private ToolsBarManager toolsBarPanel;
        [SerializeField] private PauseMenuController pauseMenuPanel;
        [SerializeField] private Fader faderPanel;
        [SerializeField] private GameObject howToPlayObject;

        private bool _startTimer = false;
        
        public static event Action OnPlayerAwoken;
        public static event Action<bool> OnOpenUi;
        public static event Action<PlantInfo> OnSelectSeed;
        
        public void RaiseSelectSeed(PlantInfo plantInfo) => OnSelectSeed?.Invoke(plantInfo);
        public void RaiseOpenUi(bool isOpen) => OnOpenUi?.Invoke(isOpen);

        #endregion

        #region Unity_Calls

        protected override void OnInitOnce()
        {
            base.OnInitOnce();
            goToSleepText.gameObject.SetActive(false);
            awakeText.gameObject.SetActive(false);
            inventoryPanel.InitAwake(this, toolsBarPanel);
            pauseMenuPanel.InitAwake(this);
        }

        protected override void OnAfterInitialized()
        {
            base.OnAfterInitialized();
            faderPanel.OnInit();
            pauseMenuPanel.InitStart();
            OnOpenUi?.Invoke(true);
        }

        private void Update()
        {
            if(!_startTimer) return;
            timeValueText.text = TimeManager.Instance.CurrentTime.ToString("00.00");
        }

        private void OnEnable()
        {
            playerInput.OnOpenInventoryEvent += OpenInventory;
            playerInput.OnEscapeKeyEvent += pauseMenuPanel.OpenPauseMenu;
        }
        
        private void OnDisable()
        {
            playerInput.OnOpenInventoryEvent -= OpenInventory;
            playerInput.OnEscapeKeyEvent -= pauseMenuPanel.OpenPauseMenu;
        }


        #endregion
        
        public void PopulateInventory(List<PlantInfo> cropInfos)
        {
            toolsBarPanel.Init();
            inventoryPanel.PopulateInventory(cropInfos);
            cropUiController.PopulateCrops(cropInfos);
            inventoryPanel.ForceCloseInventory();
        }

        public async UniTask GoToSleep(int currentDay)
        {
            await FadeIn();
            await ShowSleepTexts(currentDay);
            dayValueText.text = currentDay.ToString("00");

            await UniTask.Yield();
            
            OnPlayerAwoken?.Invoke();
            BeamManager.Instance.InventoryManager.UpdateCropInfos().Forget();
            await FadeOut();
        }

        private async UniTask ShowSleepTexts(int currentDay)
        {
            goToSleepText.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            goToSleepText.gameObject.SetActive(false);
            
            awakeText.gameObject.SetActive(true);
            awakeText.text = $"Current day: {currentDay}";
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            awakeText.gameObject.SetActive(false);
        }

        public async UniTask FadeIn()
        {
            await faderPanel.FadeIn();
        }

        public async UniTask FadeOut()
        {
            await faderPanel.FadeOut();
        }
            
        public void OpenInventory()
        {
            inventoryPanel.OpenInventory();
            if(!inventoryPanel.gameObject.activeSelf)
            {
                BeamManager.Instance.InventoryManager.UpdateCropInfos().Forget();
            }
        }

        public void OnCloseHowToPlay()
        {
            _startTimer = true;
            howToPlayObject.SetActive(false);
            OnOpenUi?.Invoke(false);
        }
        
    }
}