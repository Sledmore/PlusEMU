namespace Plus.Communication.Packets.Outgoing.BuildersClub
{
    class BuildersClubMembershipComposer : ServerPacket
    {
        public BuildersClubMembershipComposer()
            : base(ServerPacketHeader.BuildersClubMembershipMessageComposer)
        {
            WriteInteger(int.MaxValue);
            WriteInteger(100);
            WriteInteger(0);
            WriteInteger(int.MaxValue);
        }
    }
}
