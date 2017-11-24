using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Groups
{
    public class GroupBadgeParts
    {
        public int Id { get; private set; }
        public string AssetOne { get; private set; }
        public string AssetTwo { get; private set; }

        public GroupBadgeParts(int id, string assetOne, string assetTwo)
        {
            this.Id = id;
            this.AssetOne = assetOne;
            this.AssetTwo = assetTwo;
        }
    }
}
