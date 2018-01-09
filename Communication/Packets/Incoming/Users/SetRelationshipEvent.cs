using System;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Relationships;

using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.HabboHotel.Users.Messenger;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Users
{
    class SetRelationshipEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null)
                return;

            int user = packet.PopInt();
            int type = packet.PopInt();

            if (!session.GetHabbo().GetMessenger().FriendshipExists(user))
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Oops, you can only set a relationship where a friendship exists."));
                return;
            }

            if (type < 0 || type > 3)
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Oops, you've chosen an invalid relationship type."));
                return;
            }

            if (session.GetHabbo().Relationships.Count > 2000)
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Sorry, you're limited to a total of 2000 relationships."));
                return;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (type == 0)
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", user);

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", user);
                    dbClient.RunQuery();

                    if (session.GetHabbo().Relationships.ContainsKey(user))
                        session.GetHabbo().Relationships.Remove(user);
                }
                else
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", user);
                    int id = dbClient.GetInteger();

                    if (id > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                        dbClient.AddParameter("target", user);
                        dbClient.RunQuery();

                        if (session.GetHabbo().Relationships.ContainsKey(id))
                            session.GetHabbo().Relationships.Remove(id);
                    }

                    dbClient.SetQuery("INSERT INTO `user_relationships` (`user_id`,`target`,`type`) VALUES ('" + session.GetHabbo().Id + "', @target, @type)");
                    dbClient.AddParameter("target", user);
                    dbClient.AddParameter("type", type);
                    int newId = Convert.ToInt32(dbClient.InsertQuery());

                    if (!session.GetHabbo().Relationships.ContainsKey(user))
                        session.GetHabbo().Relationships.Add(user, new Relationship(newId, user, type));
                }

                GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(user);
                if (client != null)
                    session.GetHabbo().GetMessenger().UpdateFriend(user, client, true);
                else
                {
                    Habbo habbo = PlusEnvironment.GetHabboById(user);
                    if (habbo != null)
                    {
                        if (session.GetHabbo().GetMessenger().TryGetFriend(user, out MessengerBuddy buddy))
                            session.SendPacket(new FriendListUpdateComposer(session, buddy));
                    }
                }
            }
        }
    }
}