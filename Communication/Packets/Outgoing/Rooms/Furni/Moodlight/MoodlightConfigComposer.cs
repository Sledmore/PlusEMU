using Plus.HabboHotel.Items.Data.Moodlight;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Moodlight
{
    class MoodlightConfigComposer : MessageComposer
    {
        public MoodlightData Data { get; }

        public MoodlightConfigComposer(MoodlightData MoodlightData)
            : base(ServerPacketHeader.MoodlightConfigMessageComposer)
        {
            this.Data = MoodlightData;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Data.Presets.Count);
            packet.WriteInteger(Data.CurrentPreset);

            int i = 1;
            foreach (MoodlightPreset Preset in Data.Presets)
            {
                packet.WriteInteger(i);
                packet.WriteInteger(Preset.BackgroundOnly ? 2 : 1);
                packet.WriteString(Preset.ColorCode);
                packet.WriteInteger(Preset.ColorIntensity);
                i++;
            }
        }
    }
}