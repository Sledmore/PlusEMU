using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(GameClient session, ClientPacket packet);
    }
}