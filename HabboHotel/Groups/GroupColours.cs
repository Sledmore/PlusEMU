using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Groups
{
    public class GroupColours
    {
        public int Id { get; private set; }
        public string Colour { get; private set; }

        public GroupColours(int id, string colour)
        {
            this.Id = id;
            this.Colour = colour;
        }
    }
}
