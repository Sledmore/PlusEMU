using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetGroupCreationWindowEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            List<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(session.GetHabbo().Id).Where(x => x.Group == null).ToList();

            session.SendPacket(new GroupCreationWindowComposer(rooms));
        }
    }
}
