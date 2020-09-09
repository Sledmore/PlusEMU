using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserRoomVisitsComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public Dictionary<double, RoomData> Visits { get; }

        public ModeratorUserRoomVisitsComposer(Habbo Data, Dictionary<double, RoomData> Visits)
            : base(ServerPacketHeader.ModeratorUserRoomVisitsMessageComposer)
        {
            this.Habbo = Data;
            this.Visits = Visits;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Habbo.Id);
            packet.WriteString(Habbo.Username);
            packet.WriteInteger(Visits.Count);

            foreach (KeyValuePair<double, RoomData> Visit in Visits)
            {
                packet.WriteInteger(Visit.Value.Id);
                packet.WriteString(Visit.Value.Name);
                packet.WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Hour);
                packet.WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Minute);
            }
        }
    }
}
