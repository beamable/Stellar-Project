using Farm.Helpers;
using Farm.Managers;
using UnityEngine;

namespace Farm.Managers
{
    public class PlantingBlock : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer soilSpriteRenderer;
        [SerializeField] private Sprite soilBarrenSprite, soilPloughedSprite, wateredSprite;
        
        [Header("Crop")]
        [SerializeField] private SpriteRenderer cropSpriteRenderer;

        private bool _isWatered, _isRipe, _canPlant = true;
        private PlantInfo plantInfo;
        
        public bool IsWatered => _isWatered;
        public bool IsRipe => _isRipe;
        public bool CanPlant => _canPlant;
        public GameConstants.SoilStage CurrentStage { get; private set; }

        public void Init()
        {
            CurrentStage = GameConstants.SoilStage.Barren;
            soilSpriteRenderer.enabled = true;
            soilSpriteRenderer.sprite = null;
            cropSpriteRenderer.enabled = false;
        }

        public void PreventPlanting()
        {
            _canPlant = false;
            soilSpriteRenderer.enabled = false;
            cropSpriteRenderer.enabled = false;
        }

        #region Soil Stages

        public void PloughSoil()
        {
            if (CurrentStage != GameConstants.SoilStage.Barren) return;
            AdvanceStage();
        }

        public void SeedSoil(PlantInfo plantInfo)
        {
            this.plantInfo = plantInfo;
            if(CurrentStage != GameConstants.SoilStage.Ploughed) return;
            AdvanceStage();
        }
        
        public void WaterSoil()
        {
            if(CurrentStage is < GameConstants.SoilStage.Planted or GameConstants.SoilStage.Ripe) return;
            SetIsWatered(true);
        }

        public void PreGrowSoil()
        {
            if(CurrentStage != GameConstants.SoilStage.Planted) return;
            if(!_isWatered) return;
            AdvanceStage();
        }
        
        public void GrowSoil()
        {
            if(CurrentStage != GameConstants.SoilStage.PreGrow) return;
            if(!_isWatered) return;
            AdvanceStage();
        }
        
        public void RipeSoil()
        {
            if(CurrentStage != GameConstants.SoilStage.Growing) return;
            if(!_isWatered) return;
            AdvanceStage();
        }
        
        public bool HarvestSoil()
        {
            if(!_isRipe) return false;
            AdvanceStage();
            return true;
        }
        

        #endregion

        [ContextMenu("Advance Stage")]
        public void AdvanceStage()
        {
            if(!_canPlant) return;
            
            CurrentStage = CurrentStage == GameConstants.SoilStage.Ripe
                ? GameConstants.SoilStage.Ploughed
                : CurrentStage + 1;
            if (CurrentStage == GameConstants.SoilStage.Barren) _isWatered = false;
            SetSoilSprite();
            AdvancePlant();
        }
        
        private void SetSoilSprite()
        {
            soilSpriteRenderer.sprite = CurrentStage == GameConstants.SoilStage.Barren ? 
                soilBarrenSprite : soilPloughedSprite;
            
            if(_isWatered) soilSpriteRenderer.sprite = wateredSprite;
        }
        
        public void SetIsWatered(bool isWatered)
        {
            _isWatered = isWatered;
            SetSoilSprite();
        }

        private void AdvancePlant()
        {
            if (plantInfo == null)
            {
                cropSpriteRenderer.enabled = false;
                return;
            }
            
            switch (CurrentStage)
            {
                case GameConstants.SoilStage.Barren : 
                case GameConstants.SoilStage.Ploughed:
                    _isRipe = false;
                    cropSpriteRenderer.enabled = false;
                    break;
                case GameConstants.SoilStage.Planted:
                    cropSpriteRenderer.enabled = true;
                    cropSpriteRenderer.sprite = plantInfo.cropData.cropPlantedSprite;
                    break;
                case GameConstants.SoilStage.PreGrow:
                    cropSpriteRenderer.sprite = plantInfo.cropData.cropWateredSprite;
                    break;
                case GameConstants.SoilStage.Growing:
                    cropSpriteRenderer.sprite = plantInfo.cropData.cropGrowingSprite;
                    break;
                case GameConstants.SoilStage.Ripe:
                    _isRipe = true;
                    cropSpriteRenderer.sprite = plantInfo.cropData.cropRipeSprite;
                    break;
            }
        }

        
    }
}