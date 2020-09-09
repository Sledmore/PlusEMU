namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemOkComposer : MessageComposer
    {
        public VoucherRedeemOkComposer()
            : base(ServerPacketHeader.VoucherRedeemOkMessageComposer)
        {
           
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString("");//productName
            packet.WriteString("");//productDescription
        }
    }
}