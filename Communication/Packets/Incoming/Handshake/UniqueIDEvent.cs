using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Handshake;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class UniqueIDEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Junk = Packet.PopString();
            string MachineId = Packet.PopString();

            Session.MachineId = MachineId;

            Session.SendPacket(new SetUniqueIdComposer(MachineId));
        }
    }
}