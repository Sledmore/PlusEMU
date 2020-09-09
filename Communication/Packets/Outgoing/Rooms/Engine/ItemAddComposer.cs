using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ItemAddComposer : MessageComposer
    {
        public Item Item { get; }

        public ItemAddComposer(Item Item)
            : base(ServerPacketHeader.ItemAddMessageComposer)
        {
            this.Item = Item;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Item.Id.ToString());
            packet.WriteInteger(Item.GetBaseItem().SpriteId);
            packet.WriteString(Item.wallCoord != null ? Item.wallCoord : string.Empty);

            ItemBehaviourUtility.GenerateWallExtradata(Item, packet);

            packet.WriteInteger(-1);
            packet.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0); // Type New R63 ('use bottom')
            packet.WriteInteger(Item.UserID);
            packet.WriteString(Item.Username);
        }
    }
}