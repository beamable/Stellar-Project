using System.Collections.Generic;
using System.Linq;
using Farm.Helpers;
using Farm.Managers;
using UnityEngine;

namespace Farm.UI
{
    public class CropsUiController : MonoBehaviour
    {
        [SerializeField] private PlantUiCard plantUiCard;
        [SerializeField] private Transform cropsContainer;
        
        private List<PlantUiCard> _cropsCards = new List<PlantUiCard>();
        
        public void PopulateCrops(List<PlantInfo> cropInfos)
        {
            for (var i = 0; i < cropInfos.Count; i++)
            {
                var card = Instantiate(plantUiCard, cropsContainer);
                card.Init(false, true, null, cropInfos[i], cropInfos[i].yieldAmount, false);
                card.transform.SetParent(cropsContainer);

                card.SetSelectedImage(false);
                card.SetIsSelectedByUI(false);
                _cropsCards.Add(card);
            }
        }

        private PlantUiCard GetCropCard(GameConstants.CropType cropType)
        {
            return _cropsCards.FirstOrDefault(card => card.CurrentPlant.cropData.cropType == cropType);
        }
    }
}