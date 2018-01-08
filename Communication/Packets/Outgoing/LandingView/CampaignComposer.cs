namespace Plus.Communication.Packets.Outgoing.LandingView
{
    class CampaignComposer : ServerPacket
    {
        public CampaignComposer(string campaignString, string campaignName)
            : base(ServerPacketHeader.CampaignMessageComposer)
        {
           WriteString(campaignString);
           WriteString(campaignName);
        }
    }
}
