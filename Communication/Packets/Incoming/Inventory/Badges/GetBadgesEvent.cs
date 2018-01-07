using Plus.Communication.Packets.Outgoing.Inventory.Badges;

namespace Plus.Communication.Packets.Incoming.Inventory.Badges
{
    class GetBadgesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new BadgesComposer(Session));
        }
    }
}
