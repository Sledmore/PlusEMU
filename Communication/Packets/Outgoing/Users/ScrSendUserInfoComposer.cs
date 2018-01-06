namespace Plus.Communication.Packets.Outgoing.Users
{
    class ScrSendUserInfoComposer : ServerPacket
    {
        public ScrSendUserInfoComposer()
            : base(ServerPacketHeader.ScrSendUserInfoMessageComposer)
        {
            WriteString("habbo_club");
            WriteInteger(0); //display days
            WriteInteger(2);
            WriteInteger(0); //display months
            WriteInteger(1);
            WriteBoolean(true); // hc
            WriteBoolean(true); // vip
            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(495);
        }
    }
}
