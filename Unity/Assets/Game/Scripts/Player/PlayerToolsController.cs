using System;
using System.Collections;
using Farm.Helpers;
using Farm.Input;
using Farm.Managers;
using Farm.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Farm.Player
{
    public class PlayerToolsController : MonoBehaviour
    {
        [SerializeField] private float toolWaitTime = 1f; 
        [SerializeField] private float toolRange = 3f;
        [SerializeField] private Transform toolIndicator;

        private bool _isUsingTool;
        private WaitForSeconds _waitToolForSeconds;
        private GameConstants.ToolType _currentTool;
        private PlayerController _player;
        private PlayerAnimation _playerAnimation;
        private SoPlayerInput _playerInput;
        private Camera _camera;
        private PlantInfo currentPlant; 
        
        public bool IsUsingTool => _isUsingTool;

        #region Unity_Calls

        public void InitAwake(PlayerController player, PlayerAnimation playerAnimation, SoPlayerInput input)
        {
            _player = player;
            _waitToolForSeconds = new WaitForSeconds(toolWaitTime);
            _playerInput = input;
            _playerAnimation = playerAnimation;
        }

        public void InitStart()
        {
            SelectTool(0);
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            SetIndicatorPosition();
        }

        public void OnInitEnable()
        {
            _playerInput.OnUseToolEvent += UseTool;
            _playerInput.OnNextToolEvent += SwitchTools;
            _playerInput.OnSelectSpecificToolEvent += SelectTool;
            UiManager.OnSelectSeed += OnSelectSeed;
        }

        public void OnDeinit()
        {
            _playerInput.OnUseToolEvent -= UseTool;
            _playerInput.OnNextToolEvent -= SwitchTools;
            _playerInput.OnSelectSpecificToolEvent -= SelectTool;
            UiManager.OnSelectSeed -= OnSelectSeed;
        }

        #endregion
        
        private void OnSelectSeed(PlantInfo plantInfo)
        {
            currentPlant = plantInfo;
        }
        
        private void UseTool()
        {
            if(_player.IsBlocked) return;
            if(_isUsingTool || _currentTool == GameConstants.ToolType.None) return;
            
            var block = PlantingManager.Instance.GetBlock(GetIndicatorV2Pos());
            
            if(block == null || !block.CanPlant) return;
            
            switch (_currentTool)
            {
                case GameConstants.ToolType.Plough:
                    if(block.CurrentStage > GameConstants.SoilStage.Barren) break;
                    StartCoroutine(WaitTool());
                    _playerAnimation.SetPloughTrigger();
                    block.PloughSoil();
                    AudioManager.Instance.PlaySfx(4);
                    break;
                case GameConstants.ToolType.Seeds:
                    if(currentPlant == null) break;
                    if(currentPlant.seedsToPlant < 1) break; //TODO: handle this better UI/UX wise
                    if(block.CurrentStage > GameConstants.SoilStage.Ploughed) break;
                    StartCoroutine(WaitTool());
                    CropManager.Instance.UseSeeds(currentPlant.cropData.cropType);
                    block.SeedSoil(currentPlant);
                    AudioManager.Instance.PlaySfx(3);
                    break;
                case GameConstants.ToolType.WateringCan:
                    if(block.CurrentStage is < GameConstants.SoilStage.Planted or GameConstants.SoilStage.Ripe) break;
                    StartCoroutine(WaitTool());
                    _playerAnimation.SetWaterTrigger();
                    block.WaterSoil();
                    AudioManager.Instance.PlaySfx(7);
                    break;
                case GameConstants.ToolType.Basket:
                    if(block.CurrentStage != GameConstants.SoilStage.Ripe) break;
                    StartCoroutine(WaitTool());
                    var harvested = block.HarvestSoil();
                    if(harvested) CropManager.Instance.AddYield(currentPlant.cropData.cropType, 0);
                    AudioManager.Instance.PlaySfx(2);
                    break;
            }
        }
        
        private void SelectTool(int toolIndex)
        {
            _currentTool = GameConstants.ToolTypeArray[toolIndex];
            PlayerController.RaiseSelectSpecificTool(toolIndex);
            
            toolIndicator.gameObject.SetActive(_currentTool != GameConstants.ToolType.None);
        }
        
        private void SwitchTools()
        {
            var count = GameConstants.ToolTypeArray.Length;
            _currentTool = (GameConstants.ToolType)(
                ((int)_currentTool + 1 + count) % count);
            PlayerController.RaiseSwitchTool();
            
            toolIndicator.gameObject.SetActive(_currentTool != GameConstants.ToolType.None);
        }

        private IEnumerator WaitTool()
        {
            _isUsingTool = true;
            _player.OnMoveEvent(Vector2.zero);
            yield return _waitToolForSeconds;
            _isUsingTool = false;
        }
        
        private void SetIndicatorPosition()
        {
            if(!toolIndicator.gameObject.activeInHierarchy) return;
            
            var mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            toolIndicator.position = new Vector3(mousePos.x, mousePos.y, 0);
            
            if (Vector2.Distance(transform.position, mousePos) > toolRange)
            {
                var direction = toolIndicator.position - transform.position;
                toolIndicator.position = transform.position + direction.normalized * toolRange;
            }
            
            toolIndicator.position = new Vector3(Mathf.FloorToInt(toolIndicator.position.x) + 0.5f, 
                Mathf.FloorToInt(toolIndicator.position.y) + 0.5f, 0);
        }
        
        private Vector2Int GetIndicatorV2Pos()
        {
            return new Vector2Int(Mathf.FloorToInt(toolIndicator.position.x - 0.5f), 
                Mathf.FloorToInt(toolIndicator.position.y - 0.5f));
        }
    }
}