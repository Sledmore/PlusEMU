using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Incoming;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetGiftWrappingConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new GiftWrappingConfigurationComposer());
        }
    }
}