using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredTriggeRconfigComposer : ServerPacket
    {
        public WiredTriggeRconfigComposer(IWiredItem box, List<int> blockedItems)
            : base(ServerPacketHeader.WiredTriggeRconfigMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(5);

            base.WriteInteger(box.SetItems.Count);
            foreach (Item item in box.SetItems.Values.ToList())
            {
                base.WriteInteger(item.Id);
            }

            base.WriteInteger(box.Item.GetBaseItem().SpriteId);
            base.WriteInteger(box.Item.Id);
            base.WriteString(box.StringData);

            base.WriteInteger(box is IWiredCycle ? 1 : 0);
            if (box is IWiredCycle Cycle)
            {
                base.WriteInteger(Cycle.Delay);
            }
            base.WriteInteger(0);
            base.WriteInteger(WiredBoxTypeUtility.GetWiredId(box.Type));
            base.WriteInteger(blockedItems.Count());
            if (blockedItems.Count() > 0)
            {
                foreach (int itemId in blockedItems.ToList())
                {
                    base.WriteInteger(itemId);
                }
            }
        }
    }
}