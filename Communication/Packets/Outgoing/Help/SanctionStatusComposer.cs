namespace Plus.Communication.Packets.Outgoing.Help
{
    class SanctionStatusComposer : MessageComposer
    {
        public SanctionStatusComposer()
            : base(ServerPacketHeader.SanctionStatusMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(false);
            packet.WriteBoolean(false);
            packet.WriteString("aaaaaaaaaaaaa");
            packet.WriteInteger(1);//Hours
            packet.WriteInteger(10);
            packet.WriteString("ccccc");
            packet.WriteString("bbb");
            packet.WriteInteger(0);
            packet.WriteString("abb");
            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteBoolean(true);//if true and second boolean is false it does something. - if false, we got banned, so true is mute
        }
    }
}