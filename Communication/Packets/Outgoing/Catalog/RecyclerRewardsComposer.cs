namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class RecyclerRewardsComposer : MessageComposer
    {
        public RecyclerRewardsComposer()
            : base(ServerPacketHeader.RecyclerRewardsMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);// Count of items
        }
    }
}