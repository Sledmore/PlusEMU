namespace Plus.Communication.Packets.Outgoing.Talents
{
    class TalentTrackLevelComposer : ServerPacket
    {
        public TalentTrackLevelComposer()
            : base(ServerPacketHeader.TalentTrackLevelMessageComposer)
        {
            base.WriteString("citizenship");
            base.WriteInteger(0);
            base.WriteInteger(4);
        }
    }
}