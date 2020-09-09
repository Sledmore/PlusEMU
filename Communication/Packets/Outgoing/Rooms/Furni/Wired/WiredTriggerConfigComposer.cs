using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredTriggerConfigComposer : MessageComposer
    {
        public IWiredItem WiredItem { get; }
        public List<int> BlockedItems { get; }

        public WiredTriggerConfigComposer(IWiredItem box, List<int> blockedItems)
            : base(ServerPacketHeader.WiredTriggerConfigMessageComposer)
        {
            this.WiredItem = box;
            this.BlockedItems = blockedItems;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(false);
            packet.WriteInteger(5);

            packet.WriteInteger(WiredItem.SetItems.Count);
            foreach (Item item in WiredItem.SetItems.Values.ToList())
            {
                packet.WriteInteger(item.Id);
            }

            packet.WriteInteger(WiredItem.Item.GetBaseItem().SpriteId);
            packet.WriteInteger(WiredItem.Item.Id);
            packet.WriteString(WiredItem.StringData);

            packet.WriteInteger(WiredItem is IWiredCycle ? 1 : 0);
            if (WiredItem is IWiredCycle Cycle)
            {
                packet.WriteInteger(Cycle.Delay);
            }
            packet.WriteInteger(0);
            packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(WiredItem.Type));
            packet.WriteInteger(BlockedItems.Count());
            if (BlockedItems.Count() > 0)
            {
                foreach (int itemId in BlockedItems.ToList())
                {
                   packet.WriteInteger(itemId);
                }
            }
        }
    }
}