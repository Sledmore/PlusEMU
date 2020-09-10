using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ItemRemoveComposer : MessageComposer
    {
        public int ItemId { get; }
        public int UserId { get; }

        public ItemRemoveComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ItemRemoveMessageComposer)
        {
            this.ItemId = Item.Id;
            this.UserId = UserId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(ItemId.ToString());
            packet.WriteBoolean(false);
            packet.WriteInteger(UserId);
        }
    }
}
