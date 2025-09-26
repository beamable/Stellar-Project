using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Managers
{
    public class Tool : MonoBehaviour
    {

        [Header("UI")] 
        [SerializeField] private Image iconImage;
        [SerializeField] private Image activeImage;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private bool _isActive;

        public ToolData ToolData { get; private set; }
        
        public void Setup(ToolData data, int index)
        {
            ToolData = data;
            keyText.text = (index + 1).ToString();
            iconImage.sprite = data.toolSprite;

            SetActive(false);
            
            gameObject.name = $"Tool_{data.toolName}";
        }
        
        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            activeImage.enabled = isActive;
            canvasGroup.alpha = isActive ? 1f : 0.9f;
        }

        public void SetSeedSprite(Sprite seedSprite)
        {
            iconImage.sprite = seedSprite;
        }
    }
}