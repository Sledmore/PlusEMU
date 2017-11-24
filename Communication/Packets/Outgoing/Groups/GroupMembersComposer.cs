using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Cache;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupMembersComposer : ServerPacket
    {
        public GroupMembersComposer(Group Group, ICollection<UserCache> Members, int MembersCount, int Page, bool Admin, int ReqType, string SearchVal)
            : base(ServerPacketHeader.GroupMembersMessageComposer)
        {
            base.WriteInteger(Group.Id);
            base.WriteString(Group.Name);
            base.WriteInteger(Group.RoomId);
            base.WriteString(Group.Badge);
            base.WriteInteger(MembersCount);

            base.WriteInteger(Members.Count);
            if (MembersCount > 0)
            {
                foreach (UserCache Data in Members)
                {
                    base.WriteInteger(Group.CreatorId == Data.Id ? 0 : Group.IsAdmin(Data.Id) ? 1 : Group.IsMember(Data.Id) ? 2 : 3);
                    base.WriteInteger(Data.Id);
                    base.WriteString(Data.Username);
                    base.WriteString(Data.Look);
                    base.WriteString(string.Empty);
                }
            }
            base.WriteBoolean(Admin);
            base.WriteInteger(14);
            base.WriteInteger(Page);
            base.WriteInteger(ReqType);
            base.WriteString(SearchVal);
        }
    }
}