namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    class AvatarAspectUpdateComposer : MessageComposer
    {
        public string Figure { get; }
        public string Gender { get; }

        public AvatarAspectUpdateComposer(string Figure, string Gender)
            : base(ServerPacketHeader.AvatarAspectUpdateMessageComposer)
        {
            this.Figure = Figure;
            this.Gender = Gender;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Figure);
            packet.WriteString(Gender);
        }
    }
}
