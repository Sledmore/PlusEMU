namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PurchaseErrorComposer : MessageComposer
    {
        public int ErrorCode { get; }

        public PurchaseErrorComposer(int ErrorCode)
            : base(ServerPacketHeader.PurchaseErrorMessageComposer)
        {
            this.ErrorCode = ErrorCode;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ErrorCode);
        }
    }
}
