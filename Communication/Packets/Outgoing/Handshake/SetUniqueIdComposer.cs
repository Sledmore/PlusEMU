namespace Plus.Communication.Packets.Outgoing.Handshake
{
    class SetUniqueIdComposer : MessageComposer
    {
        public string UniqueId { get; }

        public SetUniqueIdComposer(string Id)
            : base(ServerPacketHeader.SetUniqueIdMessageComposer)
        {
            this.UniqueId = Id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(UniqueId);
        }
    }
}
