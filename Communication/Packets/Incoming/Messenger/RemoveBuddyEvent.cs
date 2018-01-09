using System.Linq;
using Plus.HabboHotel.GameClients;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Messenger
{
    class RemoveBuddyEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null)
                return;

            int amount = packet.PopInt();
            if (amount > 100)
                amount = 100;
            else if (amount < 0)
                return;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                for (int i = 0; i < amount; i++)
                {
                    int id = packet.PopInt();

                    if (session.GetHabbo().Relationships.Count(x => x.Value.UserId == id) > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = @id AND `target` = @target OR `target` = @id AND `user_id` = @target");
                        dbClient.AddParameter("id", session.GetHabbo().Id);
                        dbClient.AddParameter("target", id);
                        dbClient.RunQuery();
                    }

                    if (session.GetHabbo().Relationships.ContainsKey(id))
                        session.GetHabbo().Relationships.Remove(id);

                    GameClient target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(id);
                    if (target != null)
                    {
                        if (target.GetHabbo().Relationships.ContainsKey(session.GetHabbo().Id))
                            target.GetHabbo().Relationships.Remove(session.GetHabbo().Id);
                    }

                    session.GetHabbo().GetMessenger().DestroyFriendship(id);
                }
            }
        }
    }
}