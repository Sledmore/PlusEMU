using Plus.HabboHotel.Groups;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class UpdateGroupIdentityEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            string name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string desc = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            if (group.CreatorId != session.GetHabbo().Id)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `name`= @name, `desc` = @desc WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("name", name);
                dbClient.AddParameter("desc", desc);
                dbClient.AddParameter("groupId", groupId);
                dbClient.RunQuery();
            }

            group.Name = name;
            group.Description = desc;

            session.SendPacket(new GroupInfoComposer(group, session));
        }
    }
}
