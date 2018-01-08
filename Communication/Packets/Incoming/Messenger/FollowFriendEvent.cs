using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class FollowFriendEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null)
                return;

            int buddyId = packet.PopInt();
            if (buddyId == 0 || buddyId == session.GetHabbo().Id)
                return;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(buddyId);
            if (client == null || client.GetHabbo() == null)
                return;

            if (!client.GetHabbo().InRoom)
            {
                session.SendPacket(new FollowFriendFailedComposer(2));
                session.GetHabbo().GetMessenger().UpdateFriend(client.GetHabbo().Id, client, true);
                return;
            }
            else if (session.GetHabbo().CurrentRoom != null && client.GetHabbo().CurrentRoom != null)
            {
                if (session.GetHabbo().CurrentRoom.RoomId == client.GetHabbo().CurrentRoom.RoomId)
                    return;
            }

            session.SendPacket(new RoomForwardComposer(client.GetHabbo().CurrentRoomId));
        }
    }
}
