using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Subscriptions
{
    public class SubscriptionData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Badge { get; set; }
        public int Credits { get; set; }
        public int Duckets { get; set; }
        public int Respects { get; set; }

        public SubscriptionData(int Id, string Name, string Badge, int Credits, int Duckets, int Respects)
        {
            this.Id = Id;
            this.Name = Name;
            this.Badge = Badge;
            this.Credits = Credits;
            this.Duckets = Duckets;
            this.Respects = Respects;
        }
    }
}
