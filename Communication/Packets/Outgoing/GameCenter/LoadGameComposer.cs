using Plus.HabboHotel.Games;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class LoadGameComposer : MessageComposer
    {
        public GameData GameData { get; }
        public string SSOTicket { get; }

        public LoadGameComposer(GameData GameData, string SSOTicket)
            : base(ServerPacketHeader.LoadGameMessageComposer)
        {
            this.GameData = GameData;
            this.SSOTicket = SSOTicket;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GameData.Id);
            packet.WriteString("1365260055982");
            packet.WriteString(GameData.ResourcePath + GameData.SWF);
            packet.WriteString("best");
            packet.WriteString("showAll");
            packet.WriteInteger(60);//FPS?
            packet.WriteInteger(10);
            packet.WriteInteger(8);
            packet.WriteInteger(6);//Asset count
            packet.WriteString("assetUrl");
            packet.WriteString(GameData.ResourcePath + GameData.Assets);
            packet.WriteString("habboHost");
            packet.WriteString("http://fuseus-private-httpd-fe-1");
            packet.WriteString("accessToken");
            packet.WriteString(SSOTicket);
            packet.WriteString("gameServerHost");
            packet.WriteString(GameData.ServerHost);
            packet.WriteString("gameServerPort");
            packet.WriteString(GameData.ServerPort);
            packet.WriteString("socketPolicyPort");
            packet.WriteString(GameData.ServerHost);
        }
    }
}
