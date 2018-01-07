namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListUpdateComposer : ServerPacket
    {
        public FurniListUpdateComposer()
            : base(ServerPacketHeader.FurniListUpdateMessageComposer)
        {

        }
    }
}
