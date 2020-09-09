namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListUpdateComposer : MessageComposer
    {
        public FurniListUpdateComposer()
            : base(ServerPacketHeader.FurniListUpdateMessageComposer)
        {

        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}
