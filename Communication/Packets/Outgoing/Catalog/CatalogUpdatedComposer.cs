namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class CatalogUpdatedComposer : ServerPacket
    {
        public CatalogUpdatedComposer()
            : base(ServerPacketHeader.CatalogUpdatedMessageComposer)
        {
            WriteBoolean(false);
        }
    }
}
