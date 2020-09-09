namespace Plus.Communication.Packets.Outgoing.BuildersClub
{
    class BuildersClubMembershipComposer : MessageComposer
    {
        public BuildersClubMembershipComposer()
            : base(ServerPacketHeader.BuildersClubMembershipMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(int.MaxValue);
            packet.WriteInteger(100);
            packet.WriteInteger(0);
            packet.WriteInteger(int.MaxValue);
        }
    }
}
