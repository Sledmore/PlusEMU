using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni
{
    class OpenGiftComposer : MessageComposer
    {
        public ItemData Data { get; }
        public string Text { get; }
        public Item Item { get; }
        public bool ItemIsInRoom { get; }

        public OpenGiftComposer(ItemData Data, string Text, Item Item, bool ItemIsInRoom)
            : base(ServerPacketHeader.OpenGiftMessageComposer)
        {
            this.Data = Data;
            this.Text = Text;
            this.Item = Item;
            this.ItemIsInRoom = ItemIsInRoom;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Data.Type.ToString());
            packet.WriteInteger(Data.SpriteId);
            packet.WriteString(Data.ItemName);
            packet.WriteInteger(Item.Id);
            packet.WriteString(Data.Type.ToString());
            packet.WriteBoolean(ItemIsInRoom);//Is it in the room?
            packet.WriteString(Text);
        }
    }
}
