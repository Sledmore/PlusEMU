namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemOkComposer : ServerPacket
    {
        public VoucherRedeemOkComposer()
            : base(ServerPacketHeader.VoucherRedeemOkMessageComposer)
        {
           WriteString("");//productName
           WriteString("");//productDescription
        }
    }
}