namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemOkComposer : ServerPacket
    {
        public VoucherRedeemOkComposer()
            : base(ServerPacketHeader.VoucherRedeemOkMessageComposer)
        {
           base.WriteString("");//productName
           base.WriteString("");//productDescription
        }
    }
}