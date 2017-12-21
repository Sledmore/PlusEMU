using Plus.Communication.Packets.Outgoing;
using Plus.HabboHotel.Navigator;

namespace Plus.HabboHotel.Rooms
{
    static class RoomAppender
    {
        public static void WriteRoom(ServerPacket packet, RoomData data, RoomPromotion promotion)
        {
            packet.WriteInteger(data.Id);
            packet.WriteString(data.Name);
            packet.WriteInteger(data.OwnerId);
            packet.WriteString(data.OwnerName);
            packet.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(data.Access));
            packet.WriteInteger(data.UsersNow);
            packet.WriteInteger(data.UsersMax);
            packet.WriteString(data.Description);
            packet.WriteInteger(data.TradeSettings);
            packet.WriteInteger(data.Score);
            packet.WriteInteger(0);//Top rated room rank.
            packet.WriteInteger(data.Category);

            packet.WriteInteger(data.Tags.Count);
            foreach (string tag in data.Tags)
            {
                packet.WriteString(tag);
            }

            int RoomType = 0;
            if (data.Group != null)
                RoomType += 2;
            if (data.Promotion != null)
                RoomType += 4;
            if (data.Type == "private")
                RoomType += 8;
            if (data.AllowPets == 1)
                RoomType += 16;
            
            if (PlusEnvironment.GetGame().GetNavigator().TryGetFeaturedRoom(data.Id, out FeaturedRoom item))
            {
                RoomType += 1;
            }

            packet.WriteInteger(RoomType);

            if (item != null)
            {
                packet.WriteString(item.Image);
            }

            if (data.Group != null)
            {
                packet.WriteInteger(data.Group == null ? 0 : data.Group.Id);
                packet.WriteString(data.Group == null ? "" : data.Group.Name);
                packet.WriteString(data.Group == null ? "" : data.Group.Badge);
            }

            if (data.Promotion != null)
            {
                packet.WriteString(promotion != null ? promotion.Name : "");
                packet.WriteString(promotion != null ? promotion.Description : "");
                packet.WriteInteger(promotion != null ? promotion.MinutesLeft : 0);
            }
        }
    }
}
