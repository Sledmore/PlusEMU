namespace Plus.Communication.Packets.Outgoing.Help
{
    class SanctionStatusComposer : ServerPacket
    {
        public SanctionStatusComposer()
            : base(ServerPacketHeader.SanctionStatusMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteBoolean(false);
            base.WriteString("aaaaaaaaaaaaa");
            base.WriteInteger(1);//Hours
            base.WriteInteger(10);
            base.WriteString("ccccc");
            base.WriteString("bbb");
            base.WriteInteger(0);
            base.WriteString("abb");
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteBoolean(true);//if true and second boolean is false it does something. - if false, we got banned, so true is mute
        }
    }
}