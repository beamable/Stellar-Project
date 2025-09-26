using Farm.Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Farm.Managers
{
    [CreateAssetMenu(fileName = "Plant", menuName = "Farm/SOs/Crop", order = 0)]
    public class CropsData : ScriptableObject
    {
        [field:SerializeField] public string cropName;
        [field:SerializeField] public int startingSeedsAmount = 5;
        [field:SerializeField] public int yield;
        [field:SerializeField] public GameConstants.CropType cropType;
        
        [Header("Sprites")]
        [field:SerializeField] public Sprite cropIcon;
        [field: SerializeField] public Sprite seedsSprite;
        [field:SerializeField] public Sprite cropPlantedSprite, cropWateredSprite, cropGrowingSprite, cropRipeSprite;

    }
}