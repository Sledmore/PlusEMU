using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ObjectRemoveComposer : MessageComposer
    {
        public Item Item { get; }
        public int UserId { get; }

        public ObjectRemoveComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ObjectRemoveMessageComposer)
        {
            this.Item = Item;
            this.UserId = UserId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Item.Id.ToString());
            packet.WriteBoolean(false);
            packet.WriteInteger(UserId);
            packet.WriteInteger(0);
        }
    }
}