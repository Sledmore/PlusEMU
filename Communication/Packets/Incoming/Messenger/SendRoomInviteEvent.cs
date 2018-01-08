using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Messenger
{
    class SendRoomInviteEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session.GetHabbo().TimeMuted > 0)
            {
                session.SendNotification("Oops, you're currently muted - you cannot send room invitations.");
                return;
            }

            int amount = packet.PopInt();
            if (amount > 500)
                return; // don't send at all

            List<int> targets = new List<int>();
            for (int i = 0; i < amount; i++)
            {
                int uid = packet.PopInt();
                if (i < 100) // limit to 100 people, keep looping until we fulfil the request though
                {
                    targets.Add(uid);
                }
            }

            string message = StringCharFilter.Escape(packet.PopString());
            if (message.Length > 121)
                message = message.Substring(0, 121);

            foreach (int userId in targets)
            {
                if (!session.GetHabbo().GetMessenger().FriendshipExists(userId))
                    continue;

                GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
                if (client == null || client.GetHabbo() == null || client.GetHabbo().AllowMessengerInvites || client.GetHabbo().AllowConsoleMessages == false)
                    continue;

                client.SendPacket(new RoomInviteComposer(session.GetHabbo().Id, message));
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `chatlogs_console_invitations` (`user_id`,`message`,`timestamp`) VALUES (@userId, @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("userId", session.GetHabbo().Id);
                dbClient.AddParameter("message", message);
                dbClient.RunQuery();
            }
        }
    }
}