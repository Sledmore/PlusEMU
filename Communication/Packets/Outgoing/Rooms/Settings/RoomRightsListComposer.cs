using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomRightsListComposer : MessageComposer
    {
        public Room Room { get; }
        public RoomRightsListComposer(Room Instance)
            : base(ServerPacketHeader.RoomRightsListMessageComposer)
        {
            this.Room = Instance;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Room.Id);

            packet.WriteInteger(Room.UsersWithRights.Count);
            foreach (int Id in Room.UsersWithRights.ToList())
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
