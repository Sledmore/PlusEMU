using System;
using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;


namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    class WiredEffectConfigComposer : MessageComposer
    {
        public IWiredItem WiredItem { get; }
        public List<int> BlockedItems { get; }

        public WiredEffectConfigComposer(IWiredItem Box, List<int> BlockedItems)
            : base(ServerPacketHeader.WiredEffectConfigMessageComposer)
        {
            this.WiredItem = Box;
            this.BlockedItems = BlockedItems;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(false);
            packet.WriteInteger(15);

            packet.WriteInteger(WiredItem.SetItems.Count);
            foreach (Item Item in WiredItem.SetItems.Values.ToList())
            {
                packet.WriteInteger(Item.Id);
            }

            packet.WriteInteger(WiredItem.Item.GetBaseItem().SpriteId);
            packet.WriteInteger(WiredItem.Item.Id);

            if (WiredItem.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                if (String.IsNullOrEmpty(WiredItem.StringData))
                    WiredItem.StringData = "Bot name;0";

                packet.WriteString(WiredItem.StringData != null ? (WiredItem.StringData.Split(';')[0]) : "");
            }
            else if (WiredItem.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                if (String.IsNullOrEmpty(WiredItem.StringData))
                    WiredItem.StringData = "0;Bot name";

                packet.WriteString(WiredItem.StringData != null ? (WiredItem.StringData.Split(';')[1]) : "");
            }
            else
            {
                packet.WriteString(WiredItem.StringData);
            }

            if (WiredItem.Type != WiredBoxType.EffectMatchPosition && WiredItem.Type != WiredBoxType.EffectMoveAndRotate && WiredItem.Type != WiredBoxType.EffectMuteTriggerer && WiredItem.Type != WiredBoxType.EffectBotFollowsUserBox)
                packet.WriteInteger(0); // Loop
            else if (WiredItem.Type == WiredBoxType.EffectMatchPosition)
            {
                if (String.IsNullOrEmpty(WiredItem.StringData))
                    WiredItem.StringData = "0;0;0";

                packet.WriteInteger(3);
                packet.WriteInteger(WiredItem.StringData != null ? int.Parse(WiredItem.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(WiredItem.StringData != null ? int.Parse(WiredItem.StringData.Split(';')[1]) : 0);
                packet.WriteInteger(WiredItem.StringData != null ? int.Parse(WiredItem.StringData.Split(';')[2]) : 0);
            }
            else if (WiredItem.Type == WiredBoxType.EffectMoveAndRotate)
            {
                if (String.IsNullOrEmpty(WiredItem.StringData))
                    WiredItem.StringData = "0;0";

                packet.WriteInteger(2);
                packet.WriteInteger(WiredItem.StringData != null ? int.Parse(WiredItem.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(WiredItem.StringData != null ? int.Parse(WiredItem.StringData.Split(';')[1]) : 0);
            }
            else if (WiredItem.Type == WiredBoxType.EffectMuteTriggerer)
            {
                if (String.IsNullOrEmpty(WiredItem.StringData))
                    WiredItem.StringData = "0;Message";

                packet.WriteInteger(1);//Count, for the time.
                packet.WriteInteger(WiredItem.StringData != null ? int.Parse(WiredItem.StringData.Split(';')[0]) : 0);
            }
            else if (WiredItem.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                packet.WriteInteger(1);//Count, for the time.
                packet.WriteInteger(WiredItem.StringData != null ? int.Parse(WiredItem.StringData.Split(';')[0]) : 0);
            }
            else if (WiredItem.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                packet.WriteInteger(WiredItem.StringData != null ? int.Parse(WiredItem.StringData.Split(';')[1]) : 0);
            }

            if (WiredItem is IWiredCycle && WiredItem.Type != WiredBoxType.EffectKickUser && WiredItem.Type != WiredBoxType.EffectMatchPosition && WiredItem.Type != WiredBoxType.EffectMoveAndRotate && WiredItem.Type != WiredBoxType.EffectSetRollerSpeed)
            {
                IWiredCycle Cycle = (IWiredCycle)WiredItem;
                packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(WiredItem.Type));
                packet.WriteInteger(0);
                packet.WriteInteger(Cycle.Delay);
            }
            else if (WiredItem.Type == WiredBoxType.EffectMatchPosition || WiredItem.Type == WiredBoxType.EffectMoveAndRotate)
            {
                IWiredCycle Cycle = (IWiredCycle)WiredItem;
                packet.WriteInteger(0);
                packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(WiredItem.Type));
                packet.WriteInteger(Cycle.Delay);
            }
            else
            {
                packet.WriteInteger(0);
                packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(WiredItem.Type));
                packet.WriteInteger(0);
            }

            packet.WriteInteger(BlockedItems.Count()); // Incompatable items loop
            if (BlockedItems.Count() > 0)
            {
                foreach (int ItemId in BlockedItems.ToList())
                    packet.WriteInteger(ItemId);
            }
        }
    }
}