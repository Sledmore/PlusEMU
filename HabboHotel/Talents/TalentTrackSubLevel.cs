using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Plus.HabboHotel.Achievements;

namespace Plus.HabboHotel.Talents
{
    public class TalentTrackSubLevel
    {
        public int Level { get; set; }
        public string Badge { get; set; }
        public int RequiredProgress { get; set; }

        public TalentTrackSubLevel(int Level, string Badge, int RequiredProgress)
        {
            this.Level = Level;
            this.Badge = Badge;
            this.RequiredProgress = RequiredProgress;
        }
    }
}
