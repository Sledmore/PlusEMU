using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Badges;

namespace Plus.HabboHotel.Items.Wired.Boxes.Conditions
{
    class IsWearingBadgeBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionIsWearingBadge; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public IsWearingBadgeBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string BadgeCode = Packet.PopString();

            this.StringData = BadgeCode;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(this.StringData))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            if (!Player.GetBadgeComponent().GetBadges().Contains(Player.GetBadgeComponent().GetBadge(this.StringData)))
                return false;

            foreach (Badge Badge in Player.GetBadgeComponent().GetBadges().ToList())
            {
                if (Badge.Slot <= 0)
                    continue;

                if (Badge.Code == this.StringData)
                    return true;
            }
            return false;
        }
    }
}
