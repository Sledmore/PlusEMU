using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class FindRandomFriendingRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Instance = PlusEnvironment.GetGame().GetRoomManager().TryGetRandomLoadedRoom();

            if (Instance != null)
                Session.SendPacket(new RoomForwardComposer(Instance.Id));
        }
    }
}
