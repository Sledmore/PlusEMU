using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Messenger
{
    class SendRoomInviteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendNotification("Oops, you're currently muted - you cannot send room invitations.");
                return;
            }

            int Amount = Packet.PopInt();
            if (Amount > 500)
                return; // don't send at all

            List<int> Targets = new List<int>();
            for (int i = 0; i < Amount; i++)
            {
                int uid = Packet.PopInt();
                if (i < 100) // limit to 100 people, keep looping until we fulfil the request though
                {
                    Targets.Add(uid);
                }
            }

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 121)
                Message = Message.Substring(0, 121);

            foreach (int UserId in Targets)
            {
                if (!Session.GetHabbo().GetMessenger().FriendshipExists(UserId))
                    continue;

                GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().AllowMessengerInvites == true || Client.GetHabbo().AllowConsoleMessages == false)
                    continue;

                Client.SendPacket(new RoomInviteComposer(Session.GetHabbo().Id, Message));
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `chatlogs_console_invitations` (`user_id`,`message`,`timestamp`) VALUES (@userId, @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("userId", Session.GetHabbo().Id);
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }
        }
    }
}