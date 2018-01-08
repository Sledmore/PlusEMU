using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni
{
    class OpenGiftComposer : ServerPacket
    {
        public OpenGiftComposer(ItemData Data, string Text, Item Item, bool ItemIsInRoom)
            : base(ServerPacketHeader.OpenGiftMessageComposer)
        {
           WriteString(Data.Type.ToString());
            WriteInteger(Data.SpriteId);
           WriteString(Data.ItemName);
            WriteInteger(Item.Id);
           WriteString(Data.Type.ToString());
            WriteBoolean(ItemIsInRoom);//Is it in the room?
           WriteString(Text);
        }
    }
}
