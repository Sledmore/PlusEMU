using System.Collections.Generic;

using Plus.HabboHotel.Users.Effects;

namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectsComposer : MessageComposer
    {
        public ICollection<AvatarEffect> Effects { get; }

        public AvatarEffectsComposer(ICollection<AvatarEffect> Effects)
            : base(ServerPacketHeader.AvatarEffectsMessageComposer)
        {
            this.Effects = Effects;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Effects.Count);

            foreach (AvatarEffect Effect in Effects)
            {
                packet.WriteInteger(Effect.SpriteId);//Effect Id
                packet.WriteInteger(0);//Type, 0 = Hand, 1 = Full
                packet.WriteInteger((int)Effect.Duration);
                packet.WriteInteger(Effect.Activated ? Effect.Quantity - 1 : Effect.Quantity);
                packet.WriteInteger(Effect.Activated ? (int)Effect.TimeLeft : -1);
                packet.WriteBoolean(false);//Permanent
            }
        }
    }
}
