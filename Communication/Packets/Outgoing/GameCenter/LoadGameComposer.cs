using Plus.HabboHotel.Games;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class LoadGameComposer : ServerPacket
    {
        public LoadGameComposer(GameData GameData, string SSOTicket)
            : base(ServerPacketHeader.LoadGameMessageComposer)
        {
            base.WriteInteger(GameData.Id);
           base.WriteString("1365260055982");
           base.WriteString(GameData.ResourcePath + GameData.SWF);
           base.WriteString("best");
           base.WriteString("showAll");
            base.WriteInteger(60);//FPS?
            base.WriteInteger(10);
            base.WriteInteger(8);
            base.WriteInteger(6);//Asset count
           base.WriteString("assetUrl");
           base.WriteString(GameData.ResourcePath + GameData.Assets);
           base.WriteString("habboHost");
           base.WriteString("http://fuseus-private-httpd-fe-1");
           base.WriteString("accessToken");
           base.WriteString(SSOTicket);
           base.WriteString("gameServerHost");
           base.WriteString(GameData.ServerHost);
           base.WriteString("gameServerPort");
           base.WriteString(GameData.ServerPort);
           base.WriteString("socketPolicyPort");
           base.WriteString(GameData.ServerHost);
        }
    }
}
