using System;

namespace Farm.Helpers
{
    public static class GameConstants
    {
        public enum FarmState {None, Farm, House}
        public enum ToolType {None, Plough, Seeds, WateringCan, Basket}
        public enum SoilStage { Barren, Ploughed, Planted, PreGrow, Growing, Ripe}
        public enum CropType{ Carrot, Tomato, Pumpkin}
        
        public static readonly ToolType[] ToolTypeArray = (ToolType[])Enum.GetValues(typeof(ToolType));

        public const string MusicVolumeParam = "MusicVolume";
        public const string SfxVolumeParam = "SfxVolume";
        
        //Custom Properties
        public const string SellingPriceProperty = "SellingPrice";
        public const string DataSourceProperty = "DataSource";
        public const string YieldHeldProperty = "YieldHeld";
    }
}