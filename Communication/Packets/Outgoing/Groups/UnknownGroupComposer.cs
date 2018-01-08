namespace Plus.Communication.Packets.Outgoing.Groups
{
    class UnknownGroupComposer : ServerPacket
    {
        public UnknownGroupComposer(int GroupId, int HabboId)
            : base(ServerPacketHeader.UnknownGroupMessageComposer)
        {
            WriteInteger(GroupId);
            WriteInteger(HabboId);
        }
    }
}