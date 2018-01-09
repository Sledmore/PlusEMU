using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class FindNewFriendsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            Room instance = PlusEnvironment.GetGame().GetRoomManager().TryGetRandomLoadedRoom();

            if (instance != null)
            {
                session.SendPacket(new FindFriendsProcessResultComposer(true));
                session.SendPacket(new RoomForwardComposer(instance.Id));
            }
            else
            {
                session.SendPacket(new FindFriendsProcessResultComposer(false));
            }
        }
    }
}
