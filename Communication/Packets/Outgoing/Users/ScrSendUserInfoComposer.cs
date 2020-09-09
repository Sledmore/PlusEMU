namespace Plus.Communication.Packets.Outgoing.Users
{
    class ScrSendUserInfoComposer : MessageComposer
    {
        public ScrSendUserInfoComposer()
            : base(ServerPacketHeader.ScrSendUserInfoMessageComposer)
        {
           
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString("habbo_club");
            packet.WriteInteger(0); //display days
            packet.WriteInteger(2);
            packet.WriteInteger(0); //display months
            packet.WriteInteger(1);
            packet.WriteBoolean(true); // hc
            packet.WriteBoolean(true); // vip
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(495);
        }
    }
}
