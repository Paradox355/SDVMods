using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SFarmer = StardewValley.Farmer;
using SGame = StardewValley.Game1;
using SDVMods.Shared;

namespace SDVMods.RelationshipTracker
{
    public enum Validation
    {
        AllValid,
        NoValid,
        NoBachelors,
        NoBachelorettes
    }

    public class ModEntry : Mod
    {
        internal ModConfig Config;
        internal VillagerConfig VillagersConfig;
        private Texture2D Pixel;
        private Texture2D Cursors;
        private BackgroundRectangle BackgroundRect;
        //private FriendshipStats[] Stats = new FriendshipStats[6];
        //private List<FriendshipStats> VillagerStats = new List<FriendshipStats>();
        private int ToGoWidth;
        private int NameWidth;
        private int NameLength;
        private int Pages = 0;
        private int CurrentPage = 0;
        private int VillagerCount = 0;
        private List<FriendshipStats> StatsList = new List<FriendshipStats>();
        private string MaxName;
        private Rectangle HeartCoords = new Rectangle(62, 770, 32, 32);
        private Rectangle RightArrowCoords = new Rectangle(365, 495, 12, 11);
        private Rectangle LeftArrowCoords = new Rectangle(352, 495, 12, 11);
        private Rectangle PortraitCoords = new Rectangle(0, 0, 64, 64);
        private bool Toggle = false;
        private ClickableTextureComponent LeftArrowButton;
        private ClickableTextureComponent RightArrowButton;

        internal ITranslationHelper I18n => Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            string startingMessage = I18n.Get("template.start", new { mod = helper.ModRegistry.ModID, folder = helper.DirectoryPath });
            //Monitor.Log(startingMessage);

            Config = helper.ReadConfig<ModConfig>();
            if (Config.DatableType != DatableType.Bachelor && Config.DatableType != DatableType.Bachelorette)
            {
                Config.DatableType = DatableType.Bachelorette;
            }

            VillagersConfig = helper.ReadJsonFile<VillagerConfig>("villagers.json");
            
            Pixel = new Texture2D(SGame.graphics.GraphicsDevice, 1, 1);
            Cursors = SGame.mouseCursors;

            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            SaveEvents.AfterLoad += ResetState;
            GameEvents.OneSecondTick += GameEvents_OneSecondTick;
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            int arrowScaleOffset = 30;

            bool isShiftPressed = Helper.Input.IsDown(SButton.LeftShift) || Helper.Input.IsDown(SButton.RightShift);

            e.Button.TryGetKeyboard(out Keys keyPressed);
            e.Button.TryGetStardewInput(out InputButton input);
            e.Button.TryGetController(out Buttons button);

            if (isShiftPressed && keyPressed.Equals(Config.ActivateKey))
            {
                Helper.Input.Suppress(Config.ActivateKey.ToSButton());
                if (Toggle)
                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;

                Config.AllVillagers = !Config.AllVillagers;

                if (Toggle)
                    ProcessAndRender();

                return;
            }

            if (input.mouseLeft || button.Equals(Config.PageLeftButton) || button.Equals(Config.PageRightButton))
            {
                if (Toggle)
                {
                    if (button.Equals(Config.PageLeftButton))
                        Helper.Input.Suppress(SButton.LeftShoulder);

                    if (button.Equals(Config.PageRightButton))
                        Helper.Input.Suppress(SButton.RightShoulder);

                    if (Config.AllVillagers == false)
                    {
                        ICursorPosition cursonPosition = Helper.Input.GetCursorPosition();
                        if (LeftArrowButton != null && Config.DatableType == DatableType.Bachelor)
                        {
                            if (new Rectangle(LeftArrowButton.bounds.X, LeftArrowButton.bounds.Y,
                                    LeftArrowButton.bounds.Right + arrowScaleOffset,
                                    LeftArrowButton.bounds.Bottom + arrowScaleOffset)
                                .Contains((int) cursonPosition.ScreenPixels.X, (int) cursonPosition.ScreenPixels.Y) 
                                || button.Equals(Config.PageLeftButton))
                            {
                                Helper.Input.Suppress(SButton.MouseLeft);
                                if (Validate(DatableType.Bachelorette) != Validation.NoBachelorettes)
                                {
                                    SGame.playSound("smallSelect");
                                    Config.DatableType = DatableType.Bachelorette;
                                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                                    ProcessAndRender();
                                }
                                else
                                {
                                    SGame.showRedMessage("You don't know any Bachelorettes!");
                                }
                            }
                        }

                        if (RightArrowButton != null && Config.DatableType == DatableType.Bachelorette)
                        {
                            if (new Rectangle(RightArrowButton.bounds.X, RightArrowButton.bounds.Y,
                                    RightArrowButton.bounds.Right + arrowScaleOffset,
                                    RightArrowButton.bounds.Bottom + arrowScaleOffset)
                                .Contains((int) cursonPosition.ScreenPixels.X, (int) cursonPosition.ScreenPixels.Y) 
                                || button.Equals(Config.PageRightButton))
                            {
                                Helper.Input.Suppress(SButton.MouseLeft);
                                if (Validate(DatableType.Bachelor) != Validation.NoBachelors)
                                {
                                    SGame.playSound("smallSelect");
                                    Config.DatableType = DatableType.Bachelor;
                                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                                    ProcessAndRender();
                                }
                                else
                                {
                                    SGame.showRedMessage("You don't know any Bachelors!");
                                }
                            }
                        }
                    }
                    else
                    {
                        ICursorPosition cursonPosition = Helper.Input.GetCursorPosition();
                        if (LeftArrowButton != null && CurrentPage > 0)
                        {
                            if (new Rectangle(LeftArrowButton.bounds.X, LeftArrowButton.bounds.Y,
                                    LeftArrowButton.bounds.Right + arrowScaleOffset,
                                    LeftArrowButton.bounds.Bottom + arrowScaleOffset)
                                .Contains((int)cursonPosition.ScreenPixels.X, (int)cursonPosition.ScreenPixels.Y)
                                || button.Equals(Config.PageLeftButton))
                            {
                                Helper.Input.Suppress(SButton.MouseLeft);
                                SGame.playSound("smallSelect");
                                CurrentPage--;
                                GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                                ProcessAndRender(CurrentPage);
                            }
                        }

                        if (RightArrowButton != null && CurrentPage != (Pages - 1))
                        {
                            if (new Rectangle(RightArrowButton.bounds.X, RightArrowButton.bounds.Y,
                                    RightArrowButton.bounds.Right + arrowScaleOffset,
                                    RightArrowButton.bounds.Bottom + arrowScaleOffset)
                                .Contains((int) cursonPosition.ScreenPixels.X, (int) cursonPosition.ScreenPixels.Y)
                                || button.Equals(Config.PageRightButton))
                            {
                                Helper.Input.Suppress(SButton.MouseLeft);
                                SGame.playSound("smallSelect");
                                CurrentPage++;
                                GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                                ProcessAndRender(CurrentPage);
                            }
                        }
                    }
                }
            }

            if (keyPressed.Equals(Config.ActivateKey) || button.Equals(Config.ActivateButton))
            {
                if (!Toggle)
                {
                    if (Config.AllVillagers == false)
                    {
                        //Monitor.Log(i18n.Get("template.key"), LogLevel.Info);
                        if (Validate(Config.DatableType, true) == Validation.NoValid)
                        {
                            SGame.showRedMessage("You don't know any eligible villagers");
                        }
                        else if (Config.DatableType == DatableType.Bachelorette &&
                                 Validate(DatableType.Bachelorette) == Validation.NoBachelorettes)
                        {
                            Config.DatableType = DatableType.Bachelor;
                            if (Validate(Config.DatableType) != Validation.NoBachelors)
                            {
                                Toggle = !Toggle;
                                ProcessAndRender();
                            }
                        }
                        else if (Config.DatableType == DatableType.Bachelor &&
                                 Validate(DatableType.Bachelor) == Validation.NoBachelors)
                        {
                            Config.DatableType = DatableType.Bachelorette;
                            if (Validate(Config.DatableType) != Validation.NoBachelorettes)
                            {
                                Toggle = !Toggle;
                                ProcessAndRender();
                            }
                        }
                        else
                        {
                            Toggle = !Toggle;
                            ProcessAndRender();
                        }
                    }
                    else
                    {
                        VillagerCount = GetVillagerCount();
                        if (VillagerCount == 0)
                        {
                            SGame.showRedMessage("You don't know any Villagers");
                        }
                        else
                        {
                            Toggle = !Toggle;
                            ProcessAndRender(CurrentPage);
                        }
                    }
                }
                else
                {
                    Toggle = !Toggle;
                    LeftArrowButton = null;
                    RightArrowButton = null;
                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                }
            }

            if (keyPressed.Equals(Config.DebugKey))
            {
                Monitor.Log("-------------");
                Monitor.Log("Villager Count: " + GetVillagerCount());
                Monitor.Log("Page Count: " + Pages);
                Monitor.Log("StatsList Count: " + StatsList.Count);
                Monitor.Log("Current Page: " + CurrentPage);
            }
        }

        public void ProcessAndRender(int page = 0)
        {
            var farmers = SGame.getAllFarmers();
            Friendship friendship;
            FriendshipStats stats;
            List<NPC> npcs = new List<NPC>();
            ToGoWidth = 0;
            NameWidth = 0;
            NameLength = 0;
            int thisPage = 0;
            int i = 0;
            MaxName = "";
            if (Config.AllVillagers == true)
            {
                MaxName = "Demetrius";
                NameWidth = (int) SGame.smallFont.MeasureString(MaxName).X;
                npcs = GetVillagers();
                npcs = npcs.OrderBy(npc => npc.displayName).ToList();
            }
            else
            {
                npcs = GetDatables(Config.DatableType);
                npcs = npcs.OrderBy(npc => npc.displayName).ToList();
            }

            StatsList.Clear();

            foreach (NPC npc in npcs)
            {
                foreach (SFarmer farmer in farmers)
                {
                    if (!farmer.friendshipData.ContainsKey(npc.getName()))
                        continue;

                    if (i > 0)
                    {
                        thisPage = i / 8;
                    }

                    if (Config.AllVillagers == false || (Config.AllVillagers == true && thisPage == CurrentPage))
                    {
                        friendship = farmer.friendshipData[npc.getName()];
                        stats = new FriendshipStats(farmer, npc, friendship);
                        if (SGame.smallFont.MeasureString(stats.Level.ToString()).X > ToGoWidth)
                        {
                            ToGoWidth = (int)SGame.smallFont.MeasureString(stats.Level.ToString()).X;
                        }
                        if ((int)SGame.smallFont.MeasureString(stats.Name).X > NameWidth)
                        {
                            NameWidth = (int)SGame.smallFont.MeasureString(stats.Name).X;
                        }
                        if (stats.Name.Length > NameLength)
                        {
                            NameLength = stats.Name.Length;
                            if (Config.AllVillagers == false)
                            {
                                MaxName = stats.Name;
                            }
                        }

                        StatsList.Add(stats);
                    }

                    i++;
                }
            }
            StatsList.Sort();
            GraphicsEvents.OnPostRenderHudEvent += this.GraphicsEvents_OnPostRenderHudEvent;
        }

        private List<NPC> GetDatables(DatableType datableType)
        {
            List<NPC> datables = new List<NPC>();

            foreach (NPC character in Utility.getAllCharacters())
            {
                if (character.isVillager() && character.Gender == (int)datableType)
                    if (character.datable.Value)
                    {
                        datables.Add(character);
                    }
            }
            datables.Sort();
            datables.Reverse();
            return datables;
        }

        private List<NPC> GetVillagers()
        {
            List<NPC> villagers = new List<NPC>();
            var farmers = SGame.getAllFarmers();
            IList<PropertyInfo> props = VillagersConfig.GetType().GetProperties().ToList();

            foreach (NPC npc in Utility.getAllCharacters())
            {
                foreach (SFarmer farmer in farmers)
                {
                    if (!farmer.friendshipData.ContainsKey(npc.getName()))
                    {
                        continue;
                    }

                    foreach (var prop in props)
                    {
                        if (prop.Name == npc.Name)
                        {
                            var propValue = ObjectExtensions.GetPropValue<bool>(VillagersConfig, npc.Name);
                            if (npc.isVillager() && propValue == true)
                            {
                                villagers.Add(npc);
                            }
                        }
                    }
                        
                }
            }
            villagers.Sort();
            villagers.Reverse();
            return villagers;
        }

        public void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            int x = Config.OffsetX;
            int y = Config.OffsetY;

            int row = y + 5 + 54;
            int textX = 5;
            int textX2 = 6;

            int headingYOffset = 11;
            int portraitOffset = 0;
            int extraLineSpace = 0;
            int nameSpace = (int)SGame.smallFont.MeasureString(MaxName).X;

            if (Config.ShowPortrait)
            {
                portraitOffset = 32;
            }
            int bachelorOffset = 0;

            string heading = "Bachelorettes";
            if (Config.DatableType == DatableType.Bachelor)
            {
                bachelorOffset = 50;
                heading = "Bachelors";
            }
            if (Config.AllVillagers == true)
            {
                bachelorOffset = 20;
                heading = "All Villagers";
                extraLineSpace = 68 + 5;
            }
            Vector2 headingSpace = SGame.smallFont.MeasureString(heading);

            float heartScale = 0.8f;
            int heartWidth = (int)(32 * heartScale);
            int baseWidth = (int)SGame.smallFont.MeasureString(" | " + " | " + " to go").X;
            int width = (int)((baseWidth + headingSpace.X + NameWidth + heartWidth + ToGoWidth) * 0.8f);
            int height = 218 + 54;

            int midSpace = 0;
            string msg;
            string msgMid;
            string msgToGo;
            float headingX = ((portraitOffset + width + bachelorOffset - headingSpace.X) / 2);
            int alpha = (int)(255 * Config.BackgroundOpacity);

            if (Config.DrawBackground)
            {
                BackgroundRect = new BackgroundRectangle(x, y, portraitOffset + width + bachelorOffset, height + extraLineSpace, new Color(255, 210, 132, alpha), Game1.spriteBatch, SGame.graphics.GraphicsDevice, Pixel);
                BackgroundRect.Draw();
                BackgroundRect.DrawBorder();
            }
            if (heading == "Bachelors" || (Config.AllVillagers == true && CurrentPage > 0))
            {
                LeftArrowButton = new ClickableTextureComponent(new Rectangle((int)headingX - (int)(13*4.0f), y + 6, 12, 11), Cursors, LeftArrowCoords, 4f) { hoverText = "Show Bachelorettes" };
                LeftArrowButton.draw(SGame.spriteBatch);
            }
            SGame.spriteBatch.DrawString(SGame.smallFont, heading, new Vector2(headingX+1, y + headingYOffset + 1), Color.DarkGoldenrod);
            SGame.spriteBatch.DrawString(SGame.smallFont, heading, new Vector2(headingX+2, y + headingYOffset), new Color(73, 45, 51));
            if (heading == "Bachelorettes" || (Config.AllVillagers == true && Pages > 0 && CurrentPage != Pages - 1))
            {
                RightArrowButton = new ClickableTextureComponent(new Rectangle((int)headingX + (int)headingSpace.X + 8, y + 6, 12, 11), Cursors, RightArrowCoords, 4f) { hoverText = "Show Bachelors" };
                RightArrowButton.draw(SGame.spriteBatch);
            }
            
            if (StatsList != null)
            {
                int i = 0;
                foreach (FriendshipStats stats in StatsList)
                {
                    msg = stats.Name;
                    Vector2 msgSpace = SGame.smallFont.MeasureString(msg);
                    msgMid = " " + stats.Level.ToString() + "";
                    midSpace = nameSpace - (int)msgSpace.X;
                    msgToGo = " | " + stats.ToNextLevel.ToString() + " to next";
                    Vector2 msgToGoSpace = SGame.smallFont.MeasureString(msgToGo);
                    float yOffset = row;
                    if (i > 0)
                    {
                        yOffset += (int)msgSpace.Y;
                    }
                    if (Config.ShowPortrait)
                    {
                        SGame.spriteBatch.Draw(stats.Portrait.Image, new Vector2(textX2, row - 1), PortraitCoords, Color.White, 0, new Vector2(), 0.5f, SpriteEffects.None, 0);
                    }
                    SGame.spriteBatch.DrawString(SGame.smallFont, msg, new Vector2(portraitOffset + textX, row + 1), Color.DarkGoldenrod);
                    SGame.spriteBatch.DrawString(SGame.smallFont, msg, new Vector2(portraitOffset + textX2, row), new Color(73, 45, 51));
                    SGame.spriteBatch.DrawString(SGame.smallFont, msgMid, new Vector2(portraitOffset + textX + msgSpace.X + midSpace, row + 1), Color.DarkGoldenrod);
                    SGame.spriteBatch.DrawString(SGame.smallFont, msgMid, new Vector2(portraitOffset + textX2 + msgSpace.X + midSpace, row), new Color(73, 45, 51));
                    SGame.spriteBatch.Draw(SGame.menuTexture, new Vector2(portraitOffset + textX + msgSpace.X + midSpace + heartWidth * 1.2f, row + 3), HeartCoords, Color.DarkGoldenrod, 0, new Vector2(), heartScale, SpriteEffects.None, 0);
                    SGame.spriteBatch.Draw(SGame.menuTexture, new Vector2(portraitOffset + textX2 + msgSpace.X + midSpace + heartWidth * 1.2f, row + 2), HeartCoords, Color.White, 0, new Vector2(), heartScale, SpriteEffects.None, 0);
                    SGame.spriteBatch.DrawString(SGame.smallFont, msgToGo, new Vector2(portraitOffset + textX + msgSpace.X + midSpace + heartWidth + 33, row + 1), Color.DarkGoldenrod);
                    SGame.spriteBatch.DrawString(SGame.smallFont, msgToGo, new Vector2(portraitOffset + textX2 + msgSpace.X + midSpace + heartWidth + 33, row), new Color(75, 45, 51));
                    row += 1;
                    row += (int)msgSpace.Y;
                    i++;
                }
            }
        }
                
        public void ResetState(object sender, EventArgs e)
        {
            if (Toggle)
            {
                GraphicsEvents.OnPostRenderHudEvent -= GraphicsEvents_OnPostRenderHudEvent;
                Toggle = !Toggle;
            }
            Pages = 0;
            CurrentPage = 0;
            VillagerCount = 0;
            //VillagerStats.Clear();
            StatsList.Clear();
        }

        private List<FriendshipStats> GetStats()
        {
            var farmers = SGame.getAllFarmers();
            Friendship friendship;
            List<NPC> Villagers = GetVillagers();
            List<FriendshipStats> VillagerStats = new List<FriendshipStats>();
            foreach (NPC npc in Villagers)
            {
                foreach (SFarmer farmer in farmers)
                {
                    if (!farmer.friendshipData.ContainsKey(npc.getName()))
                        continue;

                    IList<PropertyInfo> props = VillagersConfig.GetType().GetProperties().ToList();
                    foreach (var prop in props)
                    {
                        if (prop.Name == npc.Name)
                        {
                            Monitor.Log("Found Json Property: " + prop.Name);
                            var propValue = ObjectExtensions.GetPropValue<bool>(VillagersConfig, npc.Name);
                            //PropertyInfo info = VillagersConfig.GetType().GetProperty(prop.Name);
                            if (propValue == true)
                            {
                                Monitor.Log("Property value is true");
                                friendship = farmer.friendshipData[npc.getName()];
                                FriendshipStats villager = new FriendshipStats(farmer, npc, friendship);
                                VillagerStats.Add(villager);
                            }
                            else
                            {
                                Monitor.Log("Property value is false");
                            }
                        }
                    }

                }
            }
            if (VillagerStats.Count > 0)
            {
                Pages = VillagerStats.Count / 8;
                if (VillagerStats.Count % 8 > 0)
                {
                    Pages++;
                }
            }
            VillagerStats.Sort();
            return VillagerStats;
        }

        private Validation Validate(DatableType datableType, bool checkAll = false, bool allVillagers = false, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            Validation validation = Validation.AllValid;
            var farmers = SGame.getAllFarmers();
            int i = 0;
            int invalidCounter = 0;
            if (allVillagers == false)
            {
                if (datableType == DatableType.Bachelor || checkAll == true)
                {
                    foreach (NPC npc in GetDatables(DatableType.Bachelor))
                    {
                        foreach (SFarmer farmer in farmers)
                        {
                            if (!farmer.friendshipData.ContainsKey(npc.getName()))
                                continue;

                            i++;
                        }
                    }
                    if (i == 0)
                    {
                        validation = Validation.NoBachelors;
                        if (checkAll)
                        {
                            invalidCounter++;
                        }
                        else
                        {
                            return validation;
                        }
                    }
                }
                i = 0;
                if (datableType == DatableType.Bachelorette || checkAll == true)
                {
                    foreach (NPC npc in GetDatables(DatableType.Bachelorette))
                    {
                        foreach (SFarmer farmer in farmers)
                        {
                            if (!farmer.friendshipData.ContainsKey(npc.getName()))
                                continue;

                            i++;
                        }
                    }
                    if (i == 0)
                    {
                        validation = Validation.NoBachelorettes;
                        if (checkAll)
                        {
                            invalidCounter++;
                        }
                        else
                        {
                            return validation;
                        }

                    }
                }
                if (checkAll == true)
                {
                    if (invalidCounter == 2)
                        validation = Validation.NoValid;
                }
            }
            else
            {
                VillagerCount = GetVillagerCount();
                validation = Validation.AllValid;
            }   
            return validation;
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Toggle)
            {
                if (Config.AllVillagers == true)
                {
                    VillagerCount = GetVillagerCount();
                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                    ProcessAndRender(CurrentPage);
                }
                else if (Config.DatableType == DatableType.Bachelor && Validate(Config.DatableType) != Validation.NoBachelors)
                {
                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                    ProcessAndRender();
                }
                else if (Config.DatableType == DatableType.Bachelorette && Validate(Config.DatableType) != Validation.NoBachelorettes)
                {
                    GraphicsEvents.OnPostRenderHudEvent -= this.GraphicsEvents_OnPostRenderHudEvent;
                    ProcessAndRender();
                }
                 
            }
        }

        private int GetVillagerCount()
        {
            var farmers = SGame.getAllFarmers();
            List<NPC> Villagers = GetVillagers();
            int i = 0;
            foreach (NPC npc in Villagers)
            {
                foreach (SFarmer farmer in farmers)
                {
                    if (!farmer.friendshipData.ContainsKey(npc.getName()))
                        continue;

                    IList<PropertyInfo> props = VillagersConfig.GetType().GetProperties().ToList();
                    foreach (var prop in props)
                    {
                        if (prop.Name == npc.Name)
                        {
                            var propValue = ObjectExtensions.GetPropValue<bool>(VillagersConfig, npc.Name);
                            if (propValue == true)
                            {
                                i++;
                            }
                            else
                            {
                                //Monitor.Log("Property value is false");
                            }
                        }
                    }
                }
            }
            if (i > 0)
            {
                Pages = i / 8;
                if (i % 8 > 0)
                {
                    Pages++;
                }
            }
            return i;
        }

        static int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
    }
}
