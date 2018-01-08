using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class GetRoomBannedUsersComposer : ServerPacket
    {
        public GetRoomBannedUsersComposer(Room instance)
            : base(ServerPacketHeader.GetRoomBannedUsersMessageComposer)
        {
            base.WriteInteger(instance.Id);

            base.WriteInteger(instance.GetBans().BannedUsers().Count);//Count
            foreach (int Id in instance.GetBans().BannedUsers().ToList())
            {
                UserCache Data = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Id);

                if (Data == null)
                {
                    base.WriteInteger(0);
                    base.WriteString("Unknown Error");
                }
                else
                {
                    base.WriteInteger(Data.Id);
                    base.WriteString(Data.Username);
                }
            }
        }
    }
}