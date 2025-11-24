using System;

namespace Farm.Helpers
{
    public static class GameConstants
    {
        //Enums
        public enum FarmState {None, Farm, House}
        public enum ToolType {None, Plough, Seeds, WateringCan, Basket}
        public enum SoilStage { Barren, Ploughed, Planted, PreGrow, Growing, Ripe}
        public enum CropType{ Carrot, Tomato, Pumpkin}
        
        public static readonly ToolType[] ToolTypeArray = (ToolType[])Enum.GetValues(typeof(ToolType));

        //Settings
        public const string MusicVolumeParam = "MusicVolume";
        public const string SfxVolumeParam = "SfxVolume";
        
        //Crop Inventory Properties
        public const string YieldProp = "Yield";
        public const string SeedsLeftProp = "Seeds";
    }
}