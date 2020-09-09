using System.Collections.Generic;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Cache.Type;
using System.Threading;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupMembersComposer : MessageComposer
    {
        public Group Group { get; }
        public ICollection<UserCache> Members { get; }
        public int MembersCount { get; }
        public int Page { get; }
        public bool Admin { get; }
        public int ReqType { get; }
        public string SearchVal { get; }

        public GroupMembersComposer(Group Group, ICollection<UserCache> Members, int MembersCount, int Page, bool Admin, int ReqType, string SearchVal)
            : base(ServerPacketHeader.GroupMembersMessageComposer)
        {
            this.Group = Group;
            this.Members = Members;
            this.MembersCount = MembersCount;
            this.Page = Page;
            this.Admin = Admin;
            this.ReqType = ReqType;
            this.SearchVal = SearchVal;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Group.Id);
            packet.WriteString(Group.Name);
            packet.WriteInteger(Group.RoomId);
            packet.WriteString(Group.Badge);
            packet.WriteInteger(MembersCount);

            packet.WriteInteger(Members.Count);
            if (MembersCount > 0)
            {
                foreach (UserCache Data in Members)
                {
                    packet.WriteInteger(Group.CreatorId == Data.Id ? 0 : Group.IsAdmin(Data.Id) ? 1 : Group.IsMember(Data.Id) ? 2 : 3);
                    packet.WriteInteger(Data.Id);
                    packet.WriteString(Data.Username);
                    packet.WriteString(Data.Look);
                    packet.WriteString(string.Empty);
                }
            }
            packet.WriteBoolean(Admin);
            packet.WriteInteger(14);
            packet.WriteInteger(Page);
            packet.WriteInteger(ReqType);
            packet.WriteString(SearchVal);
        }
    }
}