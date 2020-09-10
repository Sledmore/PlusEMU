using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ItemsComposer : MessageComposer
    {
        public Item[] Objects { get; }
        public int OwnerId { get; }
        public string OwnerName { get; }

        public ItemsComposer(Item[] Objects, Room Room)
            : base(ServerPacketHeader.ItemsMessageComposer)
        {
            this.Objects = Objects;
            this.OwnerId = Room.OwnerId;
            this.OwnerName = Room.OwnerName;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(OwnerId);
            packet.WriteString(OwnerName);

            packet.WriteInteger(Objects.Length);

            foreach (Item Item in Objects)
            {
                WriteWallItem(Item, OwnerId, packet);
            }
        }

        private void WriteWallItem(Item Item, int UserId, ServerPacket packet)
        {
            packet.WriteString(Item.Id.ToString());
            packet.WriteInteger(Item.Data.SpriteId);

            try
            {
                packet.WriteString(Item.wallCoord);
            }
            catch
            {
                packet.WriteString("");
            }

            ItemBehaviourUtility.GenerateWallExtradata(Item, packet);

            packet.WriteInteger(-1);
            packet.WriteInteger((Item.Data.Modes > 1) ? 1 : 0);
            packet.WriteInteger(UserId);
        }
    }
}
