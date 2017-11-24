using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    class WiredConditionConfigComposer : ServerPacket
    {
        public WiredConditionConfigComposer(IWiredItem Box)
            : base(ServerPacketHeader.WiredConditionConfigMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(5);

            base.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                base.WriteInteger(Item.Id);
            }

            base.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            base.WriteInteger(Box.Item.Id);
           base.WriteString(Box.StringData);

            if (Box.Type == WiredBoxType.ConditionMatchStateAndPosition || Box.Type == WiredBoxType.ConditionDontMatchStateAndPosition)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0;0";

                base.WriteInteger(3);//Loop
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.ConditionUserCountInRoom || Box.Type == WiredBoxType.ConditionUserCountDoesntInRoom)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0";

                base.WriteInteger(2);//Loop
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 1);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 50);
            }

            if (Box.Type == WiredBoxType.ConditionFurniHasNoFurni)
                base.WriteInteger(1);

            if (Box.Type != WiredBoxType.ConditionUserCountInRoom && Box.Type != WiredBoxType.ConditionUserCountDoesntInRoom && Box.Type != WiredBoxType.ConditionFurniHasNoFurni)
                base.WriteInteger(0);
            else if (Box.Type == WiredBoxType.ConditionFurniHasNoFurni)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0";
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 50);
            }
            base.WriteInteger(0);
            base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
        }
    }
}