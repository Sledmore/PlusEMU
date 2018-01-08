using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ItemAddComposer : ServerPacket
    {
        public ItemAddComposer(Item Item)
            : base(ServerPacketHeader.ItemAddMessageComposer)
        {
           WriteString(Item.Id.ToString());
            WriteInteger(Item.GetBaseItem().SpriteId);
           WriteString(Item.wallCoord != null ? Item.wallCoord : string.Empty);

            ItemBehaviourUtility.GenerateWallExtradata(Item, this);

            WriteInteger(-1);
            WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0); // Type New R63 ('use bottom')
            WriteInteger(Item.UserID);
           WriteString(Item.Username);
        }
    }
}