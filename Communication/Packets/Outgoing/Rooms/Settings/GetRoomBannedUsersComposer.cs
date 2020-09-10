using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Cache.Type;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class GetRoomBannedUsersComposer : MessageComposer
    {
        public int RoomId { get; }
        public List<int> BannedUsers { get; }
        public GetRoomBannedUsersComposer(int RoomId, List<int> BannedUsers)
            : base(ServerPacketHeader.GetRoomBannedUsersMessageComposer)
        {
            this.RoomId = RoomId;
            this.BannedUsers = BannedUsers;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);

            packet.WriteInteger(BannedUsers.Count);//Count
            foreach (int Id in BannedUsers)
            {
                UserCache Data = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Id);

                if (Data == null)
                {
                    packet.WriteInteger(0);
                    packet.WriteString("Unknown Error");
                }
                else
                {
                    packet.WriteInteger(Data.Id);
                    packet.WriteString(Data.Username);
                }
            }
        }
    }
}