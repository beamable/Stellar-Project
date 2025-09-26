using System;
using System.Collections.Generic;
using Farm.Helpers;
using Farm.Managers;
using Farm.Player;
using UnityEngine;

namespace Farm.UI
{
    public class ToolsBarManager : MonoBehaviour
    {
        [SerializeField] private Tool toolPrefab;
        [SerializeField] private ToolData[] availableTools;
        
        private Tool _selectedTool;
        private Tool _seedTool;
        private List<Tool> _tools = new List<Tool>();
        
        public void Init()
        {
            for(int i = 0; i < GameConstants.ToolTypeArray.Length; i++ )
            {
                var data = GetToolData(GameConstants.ToolTypeArray[i]);
                var tool = Instantiate(toolPrefab, transform);
                tool.Setup(data, i);
                _tools.Add(tool);
                if(tool.ToolData.toolType == GameConstants.ToolType.Seeds) _seedTool = tool;
            }
            SelectTool(0);
        }
        
        private void OnEnable()
        {
            PlayerController.OnSwitchTool += SwitchTools;
            PlayerController.OnSelectSpecificTool += SelectTool;
        }

        private void OnDisable()
        {
            PlayerController.OnSwitchTool -= SwitchTools;
            PlayerController.OnSelectSpecificTool -= SelectTool;
        }

        private ToolData GetToolData(GameConstants.ToolType toolType)
        {
            ToolData toolData = null;
            foreach (var type in GameConstants.ToolTypeArray)
            {
                if(toolType == type)
                    toolData = availableTools[Array.IndexOf(GameConstants.ToolTypeArray, type)];
            }
            return toolData;
        }
        
        private void SelectTool(int index)
        {
            if(index < 0 || index >= _tools.Count) return;

            if(_selectedTool != null)
                _selectedTool.SetActive(false);
            _selectedTool = _tools[index];
            _selectedTool.SetActive(true);
        }
        
        private void SwitchTools()
        {
            if(_selectedTool == null) return;
            
            var currentIndex = _tools.IndexOf(_selectedTool);
            var nextIndex = (currentIndex + 1) % _tools.Count;
            
            _selectedTool.SetActive(false);
            SelectTool(nextIndex);
        }

        public void SetSeedSprite(Sprite seedSprite)
        {
            _seedTool.SetSeedSprite(seedSprite);
        }
    }
}