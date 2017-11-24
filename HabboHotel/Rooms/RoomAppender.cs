using Plus.Communication.Packets.Outgoing;
using Plus.HabboHotel.Navigator;

namespace Plus.HabboHotel.Rooms
{
    static class RoomAppender
    {
        public static void WriteRoom(ServerPacket Packet, RoomData Data, RoomPromotion Promotion, bool NewNavigator = false)
        {
            Packet.WriteInteger(Data.Id);
            Packet.WriteString(Data.Name);
            Packet.WriteInteger(Data.OwnerId);
            Packet.WriteString(Data.OwnerName);
            Packet.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Data.Access));
            Packet.WriteInteger(Data.UsersNow);
            Packet.WriteInteger(Data.UsersMax);
            Packet.WriteString(Data.Description);
            Packet.WriteInteger(Data.TradeSettings);
            Packet.WriteInteger(Data.Score);
            Packet.WriteInteger(0);//Top rated room rank.
            Packet.WriteInteger(Data.Category);

            Packet.WriteInteger(Data.Tags.Count);
            foreach (string tag in Data.Tags)
            {
                Packet.WriteString(tag);
            }

            int RoomType = 0;
            if (Data.Group != null)
                RoomType += 2;
            if (Data.Promotion != null)
                RoomType += 4;
            if (Data.Type == "private")
                RoomType += 8;
            if (Data.AllowPets == 1)
                RoomType += 16;

            FeaturedRoom Item = null;
            if (PlusEnvironment.GetGame().GetNavigator().TryGetFeaturedRoom(Data.Id, out Item))
            {
                RoomType += 1;
            }

            Packet.WriteInteger(RoomType);

            if (Item != null)
            {
                Packet.WriteString(Item.Image);
            }

            if (Data.Group != null)
            {
                Packet.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                Packet.WriteString(Data.Group == null ? "" : Data.Group.Name);
                Packet.WriteString(Data.Group == null ? "" : Data.Group.Badge);
            }

            if (Data.Promotion != null)
            {
                Packet.WriteString(Promotion != null ? Promotion.Name : "");
                Packet.WriteString(Promotion != null ? Promotion.Description : "");
                Packet.WriteInteger(Promotion != null ? Promotion.MinutesLeft : 0);
            }
        }
    }
}
