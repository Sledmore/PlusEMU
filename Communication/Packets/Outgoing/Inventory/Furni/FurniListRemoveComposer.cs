namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListRemoveComposer : ServerPacket
    {
        public FurniListRemoveComposer(int Id)
            : base(ServerPacketHeader.FurniListRemoveMessageComposer)
        {
            base.WriteInteger(Id);
        }
    }
}
