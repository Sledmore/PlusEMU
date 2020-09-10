namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class MarketplaceConfigurationComposer : MessageComposer
    {
        public MarketplaceConfigurationComposer()
            : base(ServerPacketHeader.MarketplaceConfigurationMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(true);
            packet.WriteInteger(1);//Min price.
            packet.WriteInteger(0);//1?
            packet.WriteInteger(0);//5?
            packet.WriteInteger(1);
            packet.WriteInteger(99999999);//Max price.
            packet.WriteInteger(48);
            packet.WriteInteger(7);//Days.
        }
    }
}