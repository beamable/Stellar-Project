using Farm.Helpers;
using UnityEngine;

namespace Farm.Managers
{
    [CreateAssetMenu(fileName = "Tool", menuName = "Farm/SOs/Tool", order = 0)]
    public class ToolData : ScriptableObject
    {
        [field: SerializeField] public string toolName;
        [field: SerializeField] public Sprite toolSprite;
        [field: SerializeField] public GameConstants.ToolType toolType;
        
    }
}