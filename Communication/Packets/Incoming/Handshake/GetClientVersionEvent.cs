using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Incoming;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class GetClientVersionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Build = Packet.PopString();

            if (PlusEnvironment.SWFRevision != Build)
                PlusEnvironment.SWFRevision = Build;
        }
    }
}