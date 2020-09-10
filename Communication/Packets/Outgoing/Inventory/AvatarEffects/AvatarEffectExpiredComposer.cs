using Plus.HabboHotel.Users.Effects;

namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectExpiredComposer : MessageComposer
    {
        public AvatarEffect Effect { get; }

        public AvatarEffectExpiredComposer(AvatarEffect Effect)
            : base(ServerPacketHeader.AvatarEffectExpiredMessageComposer)
        {
            this.Effect = Effect;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Effect.SpriteId);
        }
    }
}
