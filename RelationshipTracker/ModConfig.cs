using System.Dynamic;
using Microsoft.Xna.Framework.Input;

namespace SDVMods.RelationshipTracker
{
    class ModConfig
    {
        public enum DatableType
        {
            Bachelor,
            Bachelorette
        }
        public Keys debugKey { get; set; }
        public Keys activateKey { get; set; }
        public Buttons activateButton { get; set; }
        public Buttons pageLeftButton { get; set; }
        public Buttons pageRightButton { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }
        public bool allVillagers { get; set; }
        public DatableType datableType { get; set; }
        public bool showPortrait { get; set; }
        public bool drawBackground { get; set; }
        public float backgroundOpacity { get; set; }
        public bool showGifts { get; set; }

        public ModConfig()
        {
            debugKey = Keys.J;
            activateKey = Keys.R;
            activateButton = Buttons.LeftStick;
            pageLeftButton = Buttons.LeftShoulder;
            pageRightButton = Buttons.RightShoulder;
            offsetX = 2;
            offsetY = 112;
            allVillagers = false;
            datableType = DatableType.Bachelorette;
            showPortrait = true;
            drawBackground = true;
            backgroundOpacity = 1.0f;
            showGifts = false;
        }
    }
}
