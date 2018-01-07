namespace Plus.Communication.Packets.Outgoing.Groups
{
    class NewGroupInfoComposer : ServerPacket
    {
        public NewGroupInfoComposer(int RoomId, int GroupId)
            : base(ServerPacketHeader.NewGroupInfoMessageComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteInteger(GroupId);
        }
    }
}
