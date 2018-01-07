using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Users
{
    class SetMessengerInviteStatusEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            bool Status = packet.PopBoolean();

            session.GetHabbo().AllowMessengerInvites = Status;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `ignore_invites` = @MessengerInvites WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("MessengerInvites", PlusEnvironment.BoolToEnum(Status));
                dbClient.RunQuery();
            }
        }
    }
}
