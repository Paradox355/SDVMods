using Microsoft.Xna.Framework.Input;

namespace SDVMods.RelationshipTracker
{
    public enum DatableType
    {
        Bachelor,
        Bachelorette
    }

    class ModConfig
    {
        public Keys DebugKey { get; set; } = Keys.J;
        public Keys ActivateKey { get; set; } = Keys.R;
        public Buttons ActivateButton { get; set; } = Buttons.LeftStick;
        public Buttons PageLeftButton { get; set; } = Buttons.LeftShoulder;
        public Buttons PageRightButton { get; set; } = Buttons.RightShoulder;
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
