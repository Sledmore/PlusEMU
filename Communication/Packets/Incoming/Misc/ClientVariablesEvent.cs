using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class ClientVariablesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int idkYet = packet.PopInt();
            string gordanPath = packet.PopString();
            string externalVariables = packet.PopString();
        }
    }
}
