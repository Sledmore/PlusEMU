namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class GiftWrappingErrorComposer : MessageComposer
    {
        public GiftWrappingErrorComposer()
            : base(ServerPacketHeader.GiftWrappingErrorMessageComposer)
        {

        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}
