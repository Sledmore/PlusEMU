using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ItemsComposer : ServerPacket
    {
        public ItemsComposer(Item[] Objects, Room Room)
            : base(ServerPacketHeader.ItemsMessageComposer)
        {

            WriteInteger(1);
            WriteInteger(Room.OwnerId);
           WriteString(Room.OwnerName);

            WriteInteger(Objects.Length);

            foreach (Item Item in Objects)
            {
                WriteWallItem(Item, Room.OwnerId);
            }
        }

        private void WriteWallItem(Item Item, int UserId)
        {
           WriteString(Item.Id.ToString());
            WriteInteger(Item.Data.SpriteId);

            try
            {
               WriteString(Item.wallCoord);
            }
            catch
            {
               WriteString("");
            }

            ItemBehaviourUtility.GenerateWallExtradata(Item, this);

            WriteInteger(-1);
            WriteInteger((Item.Data.Modes > 1) ? 1 : 0);
            WriteInteger(UserId);
        }
    }
}
