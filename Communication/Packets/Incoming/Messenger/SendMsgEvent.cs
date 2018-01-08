using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class SendMsgEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null)
                return;

            int userId = packet.PopInt();
            if (userId == 0 || userId == session.GetHabbo().Id)
                return;

            string message = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            if (string.IsNullOrWhiteSpace(message))
                return;


            if (session.GetHabbo().TimeMuted > 0)
            {
                session.SendNotification("Oops, you're currently muted - you cannot send messages.");
                return;
            }

            session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);

        }
    }
}