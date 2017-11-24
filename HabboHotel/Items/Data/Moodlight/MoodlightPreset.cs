using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.HabboHotel.Items.Data.Moodlight
{
    public class MoodlightPreset
    {
        public bool BackgroundOnly;
        public string ColorCode;
        public int ColorIntensity;

        public MoodlightPreset(string ColorCode, int ColorIntensity, bool BackgroundOnly)
        {
            this.ColorCode = ColorCode;
            this.ColorIntensity = ColorIntensity;
            this.BackgroundOnly = BackgroundOnly;
        }
    }
}
