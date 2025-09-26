
using System;
using System.Linq;
using Farm.Helpers;
using Farm.Managers;
using Farm.UI;
using UnityEngine;

namespace Farm.Managers
{
    public class PlantingManager : MonoSingleton<PlantingManager>
    {
        [field: SerializeField] public PlantingGridController gridController;

        protected override void OnAfterInitialized()
        {
            base.OnAfterInitialized();
            gridController.OnAfterInitialized();
        }

        private void OnEnable()
        {
            UiManager.OnPlayerAwoken += GrowCrops;
        }
        
        private void OnDisable()
        {
            UiManager.OnPlayerAwoken -= GrowCrops;
        }

        public PlantingBlock GetBlock(Vector2Int position)
        {
            return gridController.GetBlock(position);
        }

        private void GrowCrops()
        {
            foreach (var block in gridController.Rows.SelectMany(row => row.blocks))
            {
                if(!block.CanPlant) continue;
                
                switch (block.CurrentStage) 
                {
                    case GameConstants.SoilStage.Barren: case GameConstants.SoilStage.Ploughed:
                        continue;
                    case GameConstants.SoilStage.Planted:
                        if (!block.IsWatered) continue;
                        block.PreGrowSoil();
                        break;
                    case GameConstants.SoilStage.PreGrow:
                        if (!block.IsWatered) continue;
                        block.GrowSoil();
                        break;
                    case GameConstants.SoilStage.Growing:
                        if (!block.IsWatered) continue;
                        block.RipeSoil();
                        break;
                }
                
                if(block.IsWatered)
                    block.SetIsWatered(false);
            }
        }
    }
}