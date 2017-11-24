using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Permissions
{
    class PermissionCommand
    {
        public string Command { get; set; }
        public int GroupId { get; set; }
        public int SubscriptionId { get; set; }

        public PermissionCommand(string Command, int GroupId, int SubscriptionId)
        {
            this.Command = Command;
            this.GroupId = GroupId;
            this.SubscriptionId = SubscriptionId;
        }
    }
}
