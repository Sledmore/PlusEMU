using System;
using System.Collections.Generic;

using Plus.HabboHotel.Items.Wired;

namespace Plus.HabboHotel.Items
{
    public class ItemData
    {
        public int Id { get; set; }
        public int SpriteId { get; set; }
        public string ItemName { get; set; }
        public string PublicName { get; set; }
        public char Type { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public double Height { get; set; }
        public bool Stackable { get; set; }
        public bool Walkable { get; set; }
        public bool IsSeat { get; set; }
        public bool AllowEcotronRecycle { get; set; }
        public bool AllowTrade { get; set; }
        public bool AllowMarketplaceSell { get; set; }
        public bool AllowGift { get; set; }
        public bool AllowInventoryStack { get; set; }
        public InteractionType InteractionType { get; set; }
        public int BehaviourData { get; set; }
        public int Modes { get; set; }
        public List<int> VendingIds { get; set; }
        public List<double> AdjustableHeights { get; set; }
        public int EffectId { get; set; }
        public WiredBoxType WiredType { get; set; }
        public bool IsRare { get; set; }
        public bool ExtraRot { get; set; }

        public ItemData(int Id, int Sprite, string Name, string PublicName, string Type, int Width, int Length, double Height, bool Stackable, bool Walkable, bool IsSeat,
            bool AllowRecycle, bool AllowTrade, bool AllowMarketplaceSell, bool AllowGift, bool AllowInventoryStack, InteractionType InteractionType, int behaviourData, int Modes,
            string VendingIds, string AdjustableHeights, int EffectId, bool IsRare, bool ExtraRot)
        {
            this.Id = Id;
            this.SpriteId = Sprite;
            this.ItemName = Name;
            this.PublicName = PublicName;
            this.Type = char.Parse(Type);
            this.Width = Width;
            this.Length = Length;
            this.Height = Height;
            this.Stackable = Stackable;
            this.Walkable = Walkable;
            this.IsSeat = IsSeat;
            this.AllowEcotronRecycle = AllowRecycle;
            this.AllowTrade = AllowTrade;
            this.AllowMarketplaceSell = AllowMarketplaceSell;
            this.AllowGift = AllowGift;
            this.AllowInventoryStack = AllowInventoryStack;
            this.InteractionType = InteractionType;
            this.BehaviourData = behaviourData;
            this.Modes = Modes;
            this.VendingIds = new List<int>();
            if (VendingIds.Contains(","))
            {
                foreach (string VendingId in VendingIds.Split(','))
                {
                    try
                    {
                        this.VendingIds.Add(int.Parse(VendingId));
                    }
                    catch
                    {
                        Console.WriteLine("Error with Item " + ItemName + " - Vending Ids");
                        continue;
                    }
                }
            }
            else if (!String.IsNullOrEmpty(VendingIds) && (int.Parse(VendingIds)) > 0)
                this.VendingIds.Add(int.Parse(VendingIds));

            this.AdjustableHeights = new List<double>();
            if (AdjustableHeights.Contains(","))
            {
                foreach (string H in AdjustableHeights.Split(','))
                {
                    this.AdjustableHeights.Add(double.Parse(H));
                }
            }
            else if (!String.IsNullOrEmpty(AdjustableHeights) && (double.Parse(AdjustableHeights)) > 0)
                this.AdjustableHeights.Add(double.Parse(AdjustableHeights));

            this.EffectId = EffectId;

            int wiredId = 0;
            if (this.InteractionType == InteractionType.WIRED_CONDITION || this.InteractionType == InteractionType.WIRED_TRIGGER || this.InteractionType == InteractionType.WIRED_EFFECT)
                wiredId = this.BehaviourData;

            this.WiredType = WiredBoxTypeUtility.FromWiredId(wiredId);

            this.IsRare = IsRare;
            this.ExtraRot = ExtraRot;
        }
    }
}