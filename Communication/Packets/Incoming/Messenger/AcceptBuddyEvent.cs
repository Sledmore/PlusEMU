using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class AcceptBuddyEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null)
                return;

            int amount = packet.PopInt();
            if (amount > 50)
                amount = 50;
            else if (amount < 0)
                return;

            for (int i = 0; i < amount; i++)
            {
                int requestId = packet.PopInt();

                if (!session.GetHabbo().GetMessenger().TryGetRequest(requestId, out MessengerRequest request))
                    continue;

                if (request.To != session.GetHabbo().Id)
                    return;

                if (!session.GetHabbo().GetMessenger().FriendshipExists(request.To))
                    session.GetHabbo().GetMessenger().CreateFriendship(request.From);

                session.GetHabbo().GetMessenger().HandleRequest(requestId);
            }
        }
    }
}
