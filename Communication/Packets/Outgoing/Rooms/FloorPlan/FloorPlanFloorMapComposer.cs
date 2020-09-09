using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    class FloorPlanFloorMapComposer : MessageComposer
    {
        public ICollection<Item> Items { get; }

        public FloorPlanFloorMapComposer(ICollection<Item> Items)
            : base(ServerPacketHeader.FloorPlanFloorMapMessageComposer)
        {
            this.Items = Items;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Items.Count);//TODO: Figure this out, it pushes the room coords, but it iterates them, x,y|x,y|x,y|and so on.
            foreach (Item Item in Items.ToList())
            {
                packet.WriteInteger(Item.GetX);
                packet.WriteInteger(Item.GetY);
            }
        }
    }
}
