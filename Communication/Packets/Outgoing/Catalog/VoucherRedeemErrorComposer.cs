namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemErrorComposer : MessageComposer
    {
        public int Type { get; }

        public VoucherRedeemErrorComposer(int Type)
            : base(ServerPacketHeader.VoucherRedeemErrorMessageComposer)
        {
            this.Type = Type;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Type.ToString());
        }
    }
}