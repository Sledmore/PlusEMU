using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Core.FigureData.Types
{
    public class Color
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public int ClubLevel { get; set; }
        public bool Selectable { get; set; }
        public string Value { get; set; }

        public Color(int id, int index, int clubLevel, bool selectable, string value)
        {
            this.Id = id;
            this.Index = index;
            this.ClubLevel = clubLevel;
            this.Selectable = selectable;
            this.Value = value;
        }
    }
}
