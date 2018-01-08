using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Users
{
    class SetUserFocusPreferenceEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            bool focusPreference = packet.PopBoolean();

            session.GetHabbo().FocusPreference = focusPreference;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `focus_preference` = @focusPreference WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("focusPreference", PlusEnvironment.BoolToEnum(focusPreference));
                dbClient.RunQuery();
            }
        }
    }
}
