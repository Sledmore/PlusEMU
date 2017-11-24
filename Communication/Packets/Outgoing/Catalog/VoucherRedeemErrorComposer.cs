namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemErrorComposer : ServerPacket
    {
        public VoucherRedeemErrorComposer(int Type)
            : base(ServerPacketHeader.VoucherRedeemErrorMessageComposer)
        {
           base.WriteString(Type.ToString());
        }
    }
}