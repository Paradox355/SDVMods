using System;
using StardewValley;
//using SFarmer = StardewValley.Farmer;
//using SGame = StardewValley.Game1;
using DatableType = SDVMods.RelationshipTracker.ModConfig.DatableType;

namespace SDVMods.RelationshipTracker
{
    internal enum Eligibility
    {
        Ineligible,
        Bachelor,
        Bechelorette
    }

    internal class FriendshipStats : IComparable<FriendshipStats>
    {
        // Contants
        private const int PointsPerLvl = 250;
        private int MaxPoints = 2500;
        private int MaxLevel = 10;

        // Instance Variables
        public FriendshipStatus Status;
        public string Name;
        public int Level;
        public int ToNextLevel;
        public int GiftsThisWeek;
        public Icons.Portrait Portrait;
        //private ModConfig.DatableType DatingType;

        // Comparitor
        public int CompareTo(FriendshipStats other)
        {
            return this.Name.CompareTo(other.Name);
        }

        // Methods
        public FriendshipStats(Farmer player, NPC npc, Friendship friendship, DatableType datableType)
        {
            if (npc.datable.Value && npc.Gender == (int)datableType)
            {
                Name = npc.displayName;
                Status = friendship.Status;
                int points = friendship.Points;
                if (points < 250)
                {
                    Level = 0;
                }
                else if (points >= MaxPoints)
                {
                    Level = 10;
                }
                else
                {
                    Level = points / PointsPerLvl;
                }

                ToNextLevel = 250 - (points % PointsPerLvl);
                GiftsThisWeek = friendship.GiftsThisWeek;
                this.Portrait = new Icons.Portrait(npc);

            }
        }

        public FriendshipStats(Farmer player, NPC npc, Friendship friendship)
        {
            if (npc.datable.Value)
            {
                MaxLevel = 8;
                if (npc.datingFarmer)
                    MaxLevel = 10;

                if (npc.getSpouse() == player)
                    MaxLevel = 12;
            }
            Name = npc.displayName;
            Status = friendship.Status;
            int points = friendship.Points;
            if (points < 250)
            {
                Level = 0;
            }

            Level = points / PointsPerLvl;
            if (Level > MaxLevel)
            {
                Level = MaxLevel;
            }

            ToNextLevel = 250 - (points % PointsPerLvl);
            GiftsThisWeek = friendship.GiftsThisWeek;
            this.Portrait = new Icons.Portrait(npc);
        }
    }
}
