namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PresentDeliverErrorMessageComposer : MessageComposer
    {
        public bool CreditError { get; }
        public bool DucketError { get; }

        public PresentDeliverErrorMessageComposer(bool CreditError, bool DucketError)
            : base(ServerPacketHeader.PresentDeliverErrorMessageComposer)
        {
            this.CreditError = CreditError;
            this.DucketError = DucketError;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(CreditError);//Do we have enough credits?
            packet.WriteBoolean(DucketError);//Do we have enough duckets?
        }
    }
}
