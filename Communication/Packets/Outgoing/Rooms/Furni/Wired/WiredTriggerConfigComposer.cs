using System.Linq;
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
            WriteBoolean(false);
            WriteInteger(5);

            WriteInteger(box.SetItems.Count);
            foreach (Item item in box.SetItems.Values.ToList())
            {
                WriteInteger(item.Id);
            }

            WriteInteger(box.Item.GetBaseItem().SpriteId);
            WriteInteger(box.Item.Id);
            WriteString(box.StringData);

            WriteInteger(box is IWiredCycle ? 1 : 0);
            if (box is IWiredCycle Cycle)
            {
                WriteInteger(Cycle.Delay);
            }
            WriteInteger(0);
            WriteInteger(WiredBoxTypeUtility.GetWiredId(box.Type));
            WriteInteger(blockedItems.Count());
            if (blockedItems.Count() > 0)
            {
                foreach (int itemId in blockedItems.ToList())
                {
                    WriteInteger(itemId);
                }
            }
        }
    }
}