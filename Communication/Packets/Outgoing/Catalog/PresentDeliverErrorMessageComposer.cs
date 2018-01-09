namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PresentDeliverErrorMessageComposer : ServerPacket
    {
        public PresentDeliverErrorMessageComposer(bool CreditError, bool DucketError)
            : base(ServerPacketHeader.PresentDeliverErrorMessageComposer)
        {
            WriteBoolean(CreditError);//Do we have enough credits?
            WriteBoolean(DucketError);//Do we have enough duckets?
        }
    }
}
