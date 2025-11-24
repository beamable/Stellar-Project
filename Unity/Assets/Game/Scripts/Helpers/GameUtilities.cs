using UnityEngine;

namespace Farm.Helpers
{
    public static class GameUtilities
    {
        public static Color ParseHtmlColor(string colorString)
        {
            ColorUtility.TryParseHtmlString(colorString, out var color);
            return color;
        }
        
        public static void SetCanvasGroup(CanvasGroup canvasGroup, bool isActive)
        {
            canvasGroup.alpha = isActive ? 1 : 0;
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }
    }
}