namespace Plus.Communication.Packets.Outgoing.BuildersClub
{
    class BuildersClubMembershipComposer : ServerPacket
    {
        public BuildersClubMembershipComposer()
            : base(ServerPacketHeader.BuildersClubMembershipMessageComposer)
        {
            base.WriteInteger(int.MaxValue);
            base.WriteInteger(100);
            base.WriteInteger(0);
            base.WriteInteger(int.MaxValue);
        }
    }
}
