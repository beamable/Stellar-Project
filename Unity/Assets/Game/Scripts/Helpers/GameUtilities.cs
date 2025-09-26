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
    }
}