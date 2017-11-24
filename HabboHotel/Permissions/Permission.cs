using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Permissions
{
    class Permission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }

        public Permission(int Id, string Name, string Description)
        {
            this.Id = Id;
            this.PermissionName = Name;
            this.Description = Description;
        }
    }
}
