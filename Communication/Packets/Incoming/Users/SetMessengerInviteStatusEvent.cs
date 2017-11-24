using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Users
{
    class SetMessengerInviteStatusEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Boolean Status = Packet.PopBoolean();

            Session.GetHabbo().AllowMessengerInvites = Status;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `ignore_invites` = @MessengerInvites WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("MessengerInvites", PlusEnvironment.BoolToEnum(Status));
                dbClient.RunQuery();
            }
        }
    }
}
