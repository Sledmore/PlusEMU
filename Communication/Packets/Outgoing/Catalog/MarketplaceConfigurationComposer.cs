namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class MarketplaceConfigurationComposer : ServerPacket
    {
        public MarketplaceConfigurationComposer()
            : base(ServerPacketHeader.MarketplaceConfigurationMessageComposer)
        {
            WriteBoolean(true);
            WriteInteger(1);//Min price.
            WriteInteger(0);//1?
            WriteInteger(0);//5?
            WriteInteger(1);
            WriteInteger(99999999);//Max price.
            WriteInteger(48);
            WriteInteger(7);//Days.
        }
    }
}