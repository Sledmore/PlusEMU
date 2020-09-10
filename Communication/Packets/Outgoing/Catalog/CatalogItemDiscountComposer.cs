namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class CatalogItemDiscountComposer : MessageComposer
    {
        public CatalogItemDiscountComposer()
            : base(ServerPacketHeader.CatalogItemDiscountMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(100);//Most you can get.
            packet.WriteInteger(6);
            packet.WriteInteger(1);
            packet.WriteInteger(1);
            packet.WriteInteger(2);//Count
            {
                packet.WriteInteger(40);
                packet.WriteInteger(99);
            }
        }
    }
}