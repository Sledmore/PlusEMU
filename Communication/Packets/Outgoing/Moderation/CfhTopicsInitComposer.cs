using System.Linq;
using System.Collections.Generic;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class CfhTopicsInitComposer : MessageComposer
    {
        public Dictionary<string, List<ModerationPresetActions>> UserActionPresets { get; }

        public CfhTopicsInitComposer(Dictionary<string, List<ModerationPresetActions>> UserActionPresets)
            : base(ServerPacketHeader.CfhTopicsInitMessageComposer)
        {

            this.UserActionPresets = UserActionPresets;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserActionPresets.Count);
            foreach (KeyValuePair<string, List<ModerationPresetActions>> Cat in UserActionPresets.ToList())
            {
                packet.WriteString(Cat.Key);
                packet.WriteInteger(Cat.Value.Count);
                foreach (ModerationPresetActions Preset in Cat.Value.ToList())
                {
                    packet.WriteString(Preset.Caption);
                    packet.WriteInteger(Preset.Id);
                    packet.WriteString(Preset.Type);
                }
            }
        }
    }
}
