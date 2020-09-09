namespace Plus.Communication.Packets.Outgoing.BuildersClub
{
    class BCBorrowedItemsComposer : MessageComposer
    {
        public BCBorrowedItemsComposer()
            : base(ServerPacketHeader.BCBorrowedItemsMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
        }
    }
}
