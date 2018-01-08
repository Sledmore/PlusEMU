using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Sound
{
    class SetSoundSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            string volume = "";
            for (int i = 0; i < 3; i++)
            {
                int vol = packet.PopInt();
                if (vol < 0 || vol > 100)
                {
                    vol = 100;
                }

                if (i < 2)
                    volume += vol + ",";
                else
                    volume += vol;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET volume = @volume WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("volume", volume);
                dbClient.RunQuery();
            }
        }
    }
}
