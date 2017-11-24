using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Users.Effects;

namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectsComposer : ServerPacket
    {
        public AvatarEffectsComposer(ICollection<AvatarEffect> Effects)
            : base(ServerPacketHeader.AvatarEffectsMessageComposer)
        {
            base.WriteInteger(Effects.Count);

            foreach (AvatarEffect Effect in Effects)
            {
                base.WriteInteger(Effect.SpriteId);//Effect Id
                base.WriteInteger(0);//Type, 0 = Hand, 1 = Full
                base.WriteInteger((int)Effect.Duration);
                base.WriteInteger(Effect.Activated ? Effect.Quantity - 1 : Effect.Quantity);
                base.WriteInteger(Effect.Activated ? (int)Effect.TimeLeft : -1);
                base.WriteBoolean(false);//Permanent
            }
        }
    }
}
