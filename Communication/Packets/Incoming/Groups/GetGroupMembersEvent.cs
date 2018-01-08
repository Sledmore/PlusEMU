using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.Cache.Type;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetGroupMembersEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            int page = packet.PopInt();
            string searchVal = packet.PopString();
            int requestType = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            List<UserCache> members = new List<UserCache>();

            switch (requestType)
            {
                case 0:
                    {
                        List<int> memberIds = group.GetAllMembers;
                        foreach (int id in memberIds.ToList())
                        {
                            UserCache groupMember = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(id);
                            if (groupMember == null)
                                continue;

                            if (!members.Contains(groupMember))
                                members.Add(groupMember);
                        }
                        break;
                    }

                case 1:
                    {
                        List<int> adminIds = group.GetAdministrators;
                        foreach (int id in adminIds.ToList())
                        {
                            UserCache groupMember = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(id);
                            if (groupMember == null)
                                continue;

                            if (!members.Contains(groupMember))
                                members.Add(groupMember);
                        }
                        break;
                    }

                case 2:
                    {
                        List<int> requestIds = group.GetRequests;
                        foreach (int id in requestIds.ToList())
                        {
                            UserCache groupMember = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(id);
                            if (groupMember == null)
                                continue;

                            if (!members.Contains(groupMember))
                                members.Add(groupMember);
                        }
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(searchVal))
                members = members.Where(x => x.Username.StartsWith(searchVal)).ToList();

            int startIndex = ((page - 1) * 14 + 14);
            int finishIndex = members.Count;

            session.SendPacket(new GroupMembersComposer(group, members.Skip(startIndex).Take(finishIndex - startIndex).ToList(), members.Count, page, (group.CreatorId == session.GetHabbo().Id || group.IsAdmin(session.GetHabbo().Id)), requestType, searchVal));
        }
    }
}