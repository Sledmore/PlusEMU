namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    class AvatarAspectUpdateComposer : ServerPacket
    {
        public AvatarAspectUpdateComposer(string Figure, string Gender)
            : base(ServerPacketHeader.AvatarAspectUpdateMessageComposer)
        {
            WriteString(Figure);
            WriteString(Gender);
        }
    }
}
