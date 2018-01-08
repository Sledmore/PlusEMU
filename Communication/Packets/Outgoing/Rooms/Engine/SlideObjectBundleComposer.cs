namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class SlideObjectBundleComposer : ServerPacket
    {
        public SlideObjectBundleComposer(int FromX, int FromY, double FromZ, int ToX, int ToY, double ToZ, int RollerId, int AvatarId, int ItemId)
            : base(ServerPacketHeader.SlideObjectBundleMessageComposer)
        {
            bool IsItem = ItemId > 0;

            WriteInteger(FromX);
            WriteInteger(FromY);
            WriteInteger(ToX);
            WriteInteger(ToY);
            WriteInteger(IsItem ? 1 : 0);

            if (IsItem)
                WriteInteger(ItemId);
            else
            {
                WriteInteger(RollerId);
                WriteInteger(2);
                WriteInteger(AvatarId);
            }

            WriteDouble(FromZ);
            WriteDouble(ToZ);

            if (IsItem)
            {
                WriteInteger(RollerId);
            }
        }
    }
}
