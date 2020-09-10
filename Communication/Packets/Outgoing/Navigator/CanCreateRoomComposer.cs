namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class CanCreateRoomComposer : MessageComposer
    {
        public bool Error { get; }
        public int MaxRoomsPerUser { get; }

        public CanCreateRoomComposer(bool error, int maxRoomsPerUser)
            : base(ServerPacketHeader.CanCreateRoomMessageComposer)
        {
            this.Error = error;
            this.MaxRoomsPerUser = maxRoomsPerUser;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error ? 1 : 0);
            packet.WriteInteger(MaxRoomsPerUser);
        }
    }
}
