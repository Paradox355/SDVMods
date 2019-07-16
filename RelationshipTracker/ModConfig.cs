using StardewModdingAPI;

namespace SDVMods.RelationshipTracker
{
    public enum DatableType
    {
        Bachelor,
        Bachelorette
    }

    class ModConfig
    {
        public SButton DebugKey { get; set; } = SButton.J;
        public SButton ActivateKey { get; set; } = SButton.R;
        public SButton ActivateButton { get; set; } = SButton.LeftStick;
        public SButton PageLeftButton { get; set; } = SButton.LeftShoulder;
        public SButton PageRightButton { get; set; } = SButton.RightShoulder;
        public int OffsetX { get; set; } = 2;
        public int OffsetY { get; set; } = 112;
        public bool AllVillagers { get; set; } = false;
        public DatableType DatableType { get; set; } = DatableType.Bachelorette;
        public bool ShowPortrait { get; set; } = true;
        public bool DrawBackground { get; set; } = true;
        public float BackgroundOpacity { get; set; } = 1;
        public bool ShowGifts { get; set; } = false;
    }
}
