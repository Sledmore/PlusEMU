using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Groups;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Groups;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class UpdateGroupIdentityEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            string Name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            string Desc = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Group.CreatorId != Session.GetHabbo().Id)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `name`= @name, `desc` = @desc WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("name", Name);
                dbClient.AddParameter("desc", Desc);
                dbClient.AddParameter("groupId", GroupId);
                dbClient.RunQuery();
            }

            Group.Name = Name;
            Group.Description = Desc;

            Session.SendPacket(new GroupInfoComposer(Group, Session));
        }
    }
}
