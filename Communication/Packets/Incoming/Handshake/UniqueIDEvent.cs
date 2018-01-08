using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Handshake;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class UniqueIdEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            packet.PopString();
            string machineId = packet.PopString();

            session.MachineId = machineId;

            session.SendPacket(new SetUniqueIdComposer(machineId));
        }
    }
}