namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class RecyclerRewardsComposer : ServerPacket
    {
        public RecyclerRewardsComposer()
            : base(ServerPacketHeader.RecyclerRewardsMessageComposer)
        {
            base.WriteInteger(0);// Count of items
        }
    }
}