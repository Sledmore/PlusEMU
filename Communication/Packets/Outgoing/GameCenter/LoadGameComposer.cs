using Plus.HabboHotel.Games;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class LoadGameComposer : ServerPacket
    {
        public LoadGameComposer(GameData GameData, string SSOTicket)
            : base(ServerPacketHeader.LoadGameMessageComposer)
        {
            WriteInteger(GameData.Id);
           WriteString("1365260055982");
           WriteString(GameData.ResourcePath + GameData.SWF);
           WriteString("best");
           WriteString("showAll");
            WriteInteger(60);//FPS?
            WriteInteger(10);
            WriteInteger(8);
            WriteInteger(6);//Asset count
           WriteString("assetUrl");
           WriteString(GameData.ResourcePath + GameData.Assets);
           WriteString("habboHost");
           WriteString("http://fuseus-private-httpd-fe-1");
           WriteString("accessToken");
           WriteString(SSOTicket);
           WriteString("gameServerHost");
           WriteString(GameData.ServerHost);
           WriteString("gameServerPort");
           WriteString(GameData.ServerPort);
           WriteString("socketPolicyPort");
           WriteString(GameData.ServerHost);
        }
    }
}
