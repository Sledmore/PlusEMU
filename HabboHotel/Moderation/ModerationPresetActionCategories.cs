using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Moderation
{
    class ModerationPresetActionCategories
    {
        public int Id { get; set; }
        public string Caption { get; set; }

        public ModerationPresetActionCategories(int Id, string Caption)
        {
            this.Id = Id;
            this.Caption = Caption;
        }
    }
}
