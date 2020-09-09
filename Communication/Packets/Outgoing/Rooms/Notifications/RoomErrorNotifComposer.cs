namespace Plus.Communication.Packets.Outgoing.Rooms.Notifications
{
    class RoomErrorNotifComposer : MessageComposer
    {
        public int Error { get; }
        public RoomErrorNotifComposer(int Error)
            : base(ServerPacketHeader.RoomErrorNotifMessageComposer)
        {
            this.Error = Error;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error);
        }
    }
}
