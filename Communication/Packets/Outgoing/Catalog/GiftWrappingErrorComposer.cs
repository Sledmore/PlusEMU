namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class GiftWrappingErrorComposer : ServerPacket
    {
        public GiftWrappingErrorComposer()
            : base(ServerPacketHeader.GiftWrappingErrorMessageComposer)
        {

        }
    }
}
