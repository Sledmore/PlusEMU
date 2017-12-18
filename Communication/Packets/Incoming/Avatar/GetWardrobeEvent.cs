using Plus.Communication.Packets.Outgoing.Avatar;

namespace Plus.Communication.Packets.Incoming.Avatar
{
    class GetWardrobeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            session.SendPacket(new WardrobeComposer(session.GetHabbo().Id));
        }
    }
}
