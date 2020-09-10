namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class OpenHelpToolComposer : MessageComposer
    {
        public OpenHelpToolComposer()
            : base(ServerPacketHeader.OpenHelpToolMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
        }
    }
}
