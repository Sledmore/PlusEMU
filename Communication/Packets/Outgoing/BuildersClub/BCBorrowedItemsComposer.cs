namespace Plus.Communication.Packets.Outgoing.BuildersClub
{
    class BCBorrowedItemsComposer : ServerPacket
    {
        public BCBorrowedItemsComposer()
            : base(ServerPacketHeader.BCBorrowedItemsMessageComposer)
        {
            WriteInteger(0);
        }
    }
}
