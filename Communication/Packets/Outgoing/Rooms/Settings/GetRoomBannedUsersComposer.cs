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
            WriteInteger(instance.Id);

            WriteInteger(instance.GetBans().BannedUsers().Count);//Count
            foreach (int Id in instance.GetBans().BannedUsers().ToList())
            {
                UserCache Data = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Id);

                if (Data == null)
                {
                    WriteInteger(0);
                    WriteString("Unknown Error");
                }
                else
                {
                    WriteInteger(Data.Id);
                    WriteString(Data.Username);
                }
            }
        }
    }
}