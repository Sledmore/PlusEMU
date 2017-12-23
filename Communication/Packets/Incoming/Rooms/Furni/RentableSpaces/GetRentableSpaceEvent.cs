using Plus.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class GetRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            int unknown = packet.PopInt();

            session.SendPacket(new RentableSpaceComposer());
        }
    }
}
