namespace Plus.Communication.Packets.Outgoing.Help
{
    class SanctionStatusComposer : ServerPacket
    {
        public SanctionStatusComposer()
            : base(ServerPacketHeader.SanctionStatusMessageComposer)
        {
            WriteBoolean(false);
            WriteBoolean(false);
            WriteString("aaaaaaaaaaaaa");
            WriteInteger(1);//Hours
            WriteInteger(10);
            WriteString("ccccc");
            WriteString("bbb");
            WriteInteger(0);
            WriteString("abb");
            WriteInteger(0);
            WriteInteger(0);
            WriteBoolean(true);//if true and second boolean is false it does something. - if false, we got banned, so true is mute
        }
    }
}