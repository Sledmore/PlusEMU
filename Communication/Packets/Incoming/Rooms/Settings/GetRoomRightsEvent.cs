using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class GetRoomRightsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room instance = session.GetHabbo().CurrentRoom;
            if (instance == null)
                return;

            if (!instance.CheckRights(session))
                return;


            session.SendPacket(new RoomRightsListComposer(instance));
        }
    }
}
