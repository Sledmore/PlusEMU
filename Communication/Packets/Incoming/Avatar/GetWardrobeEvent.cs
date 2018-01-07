using Plus.Communication.Packets.Outgoing.Avatar;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Avatar
{
    class GetWardrobeEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new WardrobeComposer(session.GetHabbo().Id));
        }
    }
}
