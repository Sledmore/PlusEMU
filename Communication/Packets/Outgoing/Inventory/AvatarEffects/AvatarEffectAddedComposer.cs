namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectAddedComposer : MessageComposer
    {
        public int SpriteId { get; }
        public int Duration { get; }

        public AvatarEffectAddedComposer(int SpriteId, int Duration)
            : base(ServerPacketHeader.AvatarEffectAddedMessageComposer)
        {
            this.SpriteId = SpriteId;
            this.Duration = Duration;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(SpriteId);
            packet.WriteInteger(0);//Types
            packet.WriteInteger(Duration);
            packet.WriteBoolean(false);//Permanent
        }
    }
}
