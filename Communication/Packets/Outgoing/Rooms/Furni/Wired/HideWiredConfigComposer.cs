namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    class HideWiredConfigComposer : MessageComposer
    {
        public HideWiredConfigComposer()
            : base(ServerPacketHeader.HideWiredConfigMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}