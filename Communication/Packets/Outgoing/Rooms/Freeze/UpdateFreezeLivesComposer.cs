namespace Plus.Communication.Packets.Outgoing.Rooms.Freeze
{
    class UpdateFreezeLivesComposer : MessageComposer
    {
        public int UserId { get; }
        public int FreezeLives { get; }

        public UpdateFreezeLivesComposer(int UserId, int FreezeLives)
            : base(ServerPacketHeader.UpdateFreezeLivesMessageComposer)
        {
            this.UserId = UserId;
            this.FreezeLives = FreezeLives;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(FreezeLives);
        }
    }
}
