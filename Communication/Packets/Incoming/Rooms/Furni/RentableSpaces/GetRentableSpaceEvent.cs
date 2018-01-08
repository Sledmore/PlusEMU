using Plus.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class GetRentableSpaceEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            packet.PopInt(); //unknown

            session.SendPacket(new RentableSpaceComposer());
        }
    }
}
