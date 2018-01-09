namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PurchaseErrorComposer : ServerPacket
    {
        public PurchaseErrorComposer(int ErrorCode)
            : base(ServerPacketHeader.PurchaseErrorMessageComposer)
        {
            WriteInteger(ErrorCode);
        }
    }
}
