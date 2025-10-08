using System;
using System.Collections.Generic;
using Farm.Beam;
using Farm.Helpers;
using Farm.UI;
using UnityEngine;

namespace Farm.Managers
{
    
    
    public class CropManager : MonoSingleton<CropManager>
    {
        [SerializeField] private CropsData[] plantsData;

        public List<PlantInfo> CropsList { get; private set; } = new List<PlantInfo>();
        public Dictionary<GameConstants.CropType, PlantInfo> CropsDictionary { get; private set; } = new Dictionary<GameConstants.CropType, PlantInfo>();

        public static event Action<PlantInfo> OnCropInfoUpdated;
        
        protected override void OnAfterInitialized()
        {
            base.OnAfterInitialized();
            
            // CropsList = new List<PlantInfo>();
            // CropsDictionary = new Dictionary<GameConstants.CropType, PlantInfo>();
            //
            // foreach (var data in plantsData)
            // {
            //     var plant = new PlantInfo
            //     {
            //         seedsToPlant = data.startingSeedsAmount,
            //         cropData = data
            //     };
            //     CropsDictionary.Add(data.cropType, plant);
            //     CropsList.Add(plant);
            // }
            CropsList = BeamManager.Instance.ContentManager.Crops;
            CropsDictionary = BeamManager.Instance.ContentManager.CropsDictionary;
            
            UiManager.Instance.PopulateInventory(CropsList);
        }

        public void UseSeeds(GameConstants.CropType cropType)
        {
            var plant = GetCropInfo(cropType);
            plant.seedsToPlant--;
            OnCropInfoUpdated?.Invoke(plant);
        }

        public void AddSeeds(GameConstants.CropType cropType, int seedsToAdd)
        {
            var plant = GetCropInfo(cropType);
            plant.seedsToPlant += seedsToAdd;
            OnCropInfoUpdated?.Invoke(plant);
        }
        
        public void AddYield(GameConstants.CropType cropType, int extraYield = 0)
        {
            var plant = GetCropInfo(cropType);
            var totalYield = extraYield + plant.cropData.yield;
            plant.yieldAmount += totalYield;
            OnCropInfoUpdated?.Invoke(plant);
        }

        public void UseYield(GameConstants.CropType cropType, int yieldToConsume)
        {
            var plant = GetCropInfo(cropType);
            if(plant.yieldAmount < 1) return; //TODO: handle this better UI/UX wise
            if(plant.yieldAmount < yieldToConsume) yieldToConsume = plant.yieldAmount;
            plant.yieldAmount -= yieldToConsume;
            OnCropInfoUpdated?.Invoke(plant);
        }
        
        
        public PlantInfo GetCropInfo(GameConstants.CropType cropType)
        {
            return CropsDictionary[cropType];
        }
    }
}
