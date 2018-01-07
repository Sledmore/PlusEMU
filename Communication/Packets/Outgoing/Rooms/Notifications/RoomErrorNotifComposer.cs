namespace Plus.Communication.Packets.Outgoing.Rooms.Notifications
{
    class RoomErrorNotifComposer : ServerPacket
    {
        public RoomErrorNotifComposer(int Error)
            : base(ServerPacketHeader.RoomErrorNotifMessageComposer)
        {
            base.WriteInteger(Error);
        }
    }
}
