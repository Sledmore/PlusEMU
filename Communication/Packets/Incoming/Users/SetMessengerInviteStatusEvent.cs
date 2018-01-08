using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Users
{
    class SetMessengerInviteStatusEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            bool status = packet.PopBoolean();

            session.GetHabbo().AllowMessengerInvites = status;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `ignore_invites` = @MessengerInvites WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("MessengerInvites", PlusEnvironment.BoolToEnum(status));
                dbClient.RunQuery();
            }
        }
    }
}
