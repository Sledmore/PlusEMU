using Plus.Communication.Packets.Outgoing.LandingView;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.LandingView
{
    class RefreshCampaignEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            try
            {
                string parseCampaings = packet.PopString();
                if (parseCampaings.Contains("gamesmaker"))
                    return;

                string campaingName = "";
                string[] parser = parseCampaings.Split(';');

                foreach (var value in parser)
                {
                    if (string.IsNullOrEmpty(value) || value.EndsWith(","))
                        continue;

                    string[] data = value.Split(',');
                    campaingName = data.Length > 1 ? data[1] : "";
                }

                session.SendPacket(new CampaignComposer(parseCampaings, campaingName));
            }
            catch
            {
                //ignored
            }
        }
    }
}