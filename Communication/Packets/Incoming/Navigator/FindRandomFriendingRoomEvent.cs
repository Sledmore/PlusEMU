using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class FindRandomFriendingRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            Room instance = PlusEnvironment.GetGame().GetRoomManager().TryGetRandomLoadedRoom();

            if (instance != null)
                session.SendPacket(new RoomForwardComposer(instance.Id));
        }
    }
}
