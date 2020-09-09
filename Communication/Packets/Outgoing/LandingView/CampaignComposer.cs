namespace Plus.Communication.Packets.Outgoing.LandingView
{
    class CampaignComposer : MessageComposer
    {
        public string CampaignString { get; }
        public string CampaignName { get; }

        public CampaignComposer(string campaignString, string campaignName)
            : base(ServerPacketHeader.CampaignMessageComposer)
        {
            this.CampaignName = campaignName;
            this.CampaignString = campaignString;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(CampaignString);
            packet.WriteString(CampaignName);
        }
    }
}
