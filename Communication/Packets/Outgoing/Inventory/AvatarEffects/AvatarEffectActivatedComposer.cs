using Plus.HabboHotel.Users.Effects;

namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectActivatedComposer : MessageComposer
    {
        public AvatarEffect Effect { get; }

        public AvatarEffectActivatedComposer(AvatarEffect Effect)
            : base(ServerPacketHeader.AvatarEffectActivatedMessageComposer)
        {
            this.Effect = Effect;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Effect.SpriteId);
            packet.WriteInteger((int)Effect.Duration);
            packet.WriteBoolean(false);//Permanent
        }
    }
}