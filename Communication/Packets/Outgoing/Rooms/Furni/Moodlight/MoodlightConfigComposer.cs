using Plus.HabboHotel.Items.Data.Moodlight;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Moodlight
{
    class MoodlightConfigComposer : ServerPacket
    {
        public MoodlightConfigComposer(MoodlightData MoodlightData)
            : base(ServerPacketHeader.MoodlightConfigMessageComposer)
        {
            WriteInteger(MoodlightData.Presets.Count);
            WriteInteger(MoodlightData.CurrentPreset);

            int i = 1;
            foreach (MoodlightPreset Preset in MoodlightData.Presets)
            {
                WriteInteger(i);
                WriteInteger(Preset.BackgroundOnly ? 2 : 1);
               WriteString(Preset.ColorCode);
                WriteInteger(Preset.ColorIntensity);
                i++;
            }
        }
    }
}