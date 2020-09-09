namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    class AvatarEffectComposer : MessageComposer
    {
        public int PlayerId { get; }
        public int EffectId { get; }

        public AvatarEffectComposer(int playerID, int effectID)
            : base(ServerPacketHeader.AvatarEffectMessageComposer)
        {
            this.PlayerId = playerID;
            this.EffectId = effectID;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(PlayerId);
            packet.WriteInteger(EffectId);
            packet.WriteInteger(0);
        }
    }
}