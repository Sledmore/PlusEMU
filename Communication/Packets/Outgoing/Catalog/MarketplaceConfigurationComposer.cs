namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class MarketplaceConfigurationComposer : ServerPacket
    {
        public MarketplaceConfigurationComposer()
            : base(ServerPacketHeader.MarketplaceConfigurationMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(1);//Min price.
            base.WriteInteger(0);//1?
            base.WriteInteger(0);//5?
            base.WriteInteger(1);
            base.WriteInteger(99999999);//Max price.
            base.WriteInteger(48);
            base.WriteInteger(7);//Days.
        }
    }
}