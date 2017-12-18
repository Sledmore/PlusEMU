using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class UpdateNavigatorSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int roomId = packet.PopInt();
            if (roomId == 0)
                return;

            RoomData data = null;
            if (!RoomFactory.TryGetData(roomId, out data))
                return;

            session.GetHabbo().HomeRoom = roomId;
            session.SendPacket(new NavigatorSettingsComposer(roomId));
        }
    }
}
