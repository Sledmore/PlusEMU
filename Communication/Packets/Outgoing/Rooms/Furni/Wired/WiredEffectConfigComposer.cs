using System;
using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;


namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    class WiredEffectConfigComposer : ServerPacket
    {
        public WiredEffectConfigComposer(IWiredItem Box, List<int> BlockedItems)
            : base(ServerPacketHeader.WiredEffectConfigMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(15);
          
            base.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                base.WriteInteger(Item.Id);
            }

            base.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            base.WriteInteger(Box.Item.Id);
           
            if(Box.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "Bot name;0";

               base.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[0]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;Bot name";

               base.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[1]) : "");
            }
            else
            {
               base.WriteString(Box.StringData);
            }

            if (Box.Type != WiredBoxType.EffectMatchPosition && Box.Type != WiredBoxType.EffectMoveAndRotate && Box.Type != WiredBoxType.EffectMuteTriggerer && Box.Type != WiredBoxType.EffectBotFollowsUserBox)
                base.WriteInteger(0); // Loop
            else if (Box.Type == WiredBoxType.EffectMatchPosition)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0;0";

                base.WriteInteger(3);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveAndRotate)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0";

                base.WriteInteger(2);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMuteTriggerer)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;Message";

                base.WriteInteger(1);//Count, for the time.
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                base.WriteInteger(1);//Count, for the time.
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }
            else if(Box.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }

            if (Box is IWiredCycle && Box.Type != WiredBoxType.EffectKickUser && Box.Type != WiredBoxType.EffectMatchPosition && Box.Type != WiredBoxType.EffectMoveAndRotate && Box.Type != WiredBoxType.EffectSetRollerSpeed)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                base.WriteInteger(0);
                base.WriteInteger(Cycle.Delay);
            }
            else if (Box.Type == WiredBoxType.EffectMatchPosition || Box.Type == WiredBoxType.EffectMoveAndRotate)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                base.WriteInteger(0);
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                base.WriteInteger(Cycle.Delay);
            }
            else
            {
                base.WriteInteger(0);
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                base.WriteInteger(0);
            }

            base.WriteInteger(BlockedItems.Count()); // Incompatable items loop
            if (BlockedItems.Count() > 0)
            {
                foreach (int ItemId in BlockedItems.ToList())
                    base.WriteInteger(ItemId);
            }
        }
    }
}