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
        public Keys DebugKey { get; set; }
        public Keys ActivateKey { get; set; }
        public Buttons ActivateButton { get; set; }
        public Buttons PageLeftButton { get; set; }
        public Buttons PageRightButton { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool AllVillagers { get; set; }
        public DatableType DatableType { get; set; }
        public bool ShowPortrait { get; set; }
        public bool DrawBackground { get; set; }
        public float BackgroundOpacity { get; set; }
        public bool ShowGifts { get; set; }

        public ModConfig()
        {
            DebugKey = Keys.J;
            ActivateKey = Keys.R;
            ActivateButton = Buttons.LeftStick;
            PageLeftButton = Buttons.LeftShoulder;
            PageRightButton = Buttons.RightShoulder;
            OffsetX = 2;
            OffsetY = 112;
            AllVillagers = false;
            DatableType = DatableType.Bachelorette;
            ShowPortrait = true;
            DrawBackground = true;
            BackgroundOpacity = 1.0f;
            ShowGifts = false;
        }
    }
}
