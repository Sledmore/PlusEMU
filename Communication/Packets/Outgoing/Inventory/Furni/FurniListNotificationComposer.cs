namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListNotificationComposer : ServerPacket
    {
        public FurniListNotificationComposer(int Id, int Type)
            : base(ServerPacketHeader.FurniListNotificationMessageComposer)
        {
            WriteInteger(1);
            WriteInteger(Type);
            WriteInteger(1);
            WriteInteger(Id);
        }
    }
}