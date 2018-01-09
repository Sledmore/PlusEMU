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
            WriteBoolean(false);
            WriteInteger(15);
          
            WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                WriteInteger(Item.Id);
            }

            WriteInteger(Box.Item.GetBaseItem().SpriteId);
            WriteInteger(Box.Item.Id);
           
            if(Box.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "Bot name;0";

               WriteString(Box.StringData != null ? (Box.StringData.Split(';')[0]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;Bot name";

               WriteString(Box.StringData != null ? (Box.StringData.Split(';')[1]) : "");
            }
            else
            {
               WriteString(Box.StringData);
            }

            if (Box.Type != WiredBoxType.EffectMatchPosition && Box.Type != WiredBoxType.EffectMoveAndRotate && Box.Type != WiredBoxType.EffectMuteTriggerer && Box.Type != WiredBoxType.EffectBotFollowsUserBox)
                WriteInteger(0); // Loop
            else if (Box.Type == WiredBoxType.EffectMatchPosition)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0;0";

                WriteInteger(3);
                WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveAndRotate)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0";

                WriteInteger(2);
                WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMuteTriggerer)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;Message";

                WriteInteger(1);//Count, for the time.
                WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                WriteInteger(1);//Count, for the time.
                WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }
            else if(Box.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }

            if (Box is IWiredCycle && Box.Type != WiredBoxType.EffectKickUser && Box.Type != WiredBoxType.EffectMatchPosition && Box.Type != WiredBoxType.EffectMoveAndRotate && Box.Type != WiredBoxType.EffectSetRollerSpeed)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                WriteInteger(0);
                WriteInteger(Cycle.Delay);
            }
            else if (Box.Type == WiredBoxType.EffectMatchPosition || Box.Type == WiredBoxType.EffectMoveAndRotate)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                WriteInteger(0);
                WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                WriteInteger(Cycle.Delay);
            }
            else
            {
                WriteInteger(0);
                WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                WriteInteger(0);
            }

            WriteInteger(BlockedItems.Count()); // Incompatable items loop
            if (BlockedItems.Count() > 0)
            {
                foreach (int ItemId in BlockedItems.ToList())
                    WriteInteger(ItemId);
            }
        }
    }
}