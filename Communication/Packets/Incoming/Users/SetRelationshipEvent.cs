using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

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
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int User = Packet.PopInt();
            int Type = Packet.PopInt();

            if (!Session.GetHabbo().GetMessenger().FriendshipExists(User))
            {
                Session.SendPacket(new BroadcastMessageAlertComposer("Oops, you can only set a relationship where a friendship exists."));
                return;
            }

            if (Type < 0 || Type > 3)
            {
                Session.SendPacket(new BroadcastMessageAlertComposer("Oops, you've chosen an invalid relationship type."));
                return;
            }

            if (Session.GetHabbo().Relationships.Count > 2000)
            {
                Session.SendPacket(new BroadcastMessageAlertComposer("Sorry, you're limited to a total of 2000 relationships."));
                return;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (Type == 0)
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    int Id = dbClient.GetInteger();

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    dbClient.RunQuery();

                    if (Session.GetHabbo().Relationships.ContainsKey(User))
                        Session.GetHabbo().Relationships.Remove(User);
                }
                else
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    int Id = dbClient.GetInteger();

                    if (Id > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                        dbClient.AddParameter("target", User);
                        dbClient.RunQuery();

                        if (Session.GetHabbo().Relationships.ContainsKey(Id))
                            Session.GetHabbo().Relationships.Remove(Id);
                    }

                    dbClient.SetQuery("INSERT INTO `user_relationships` (`user_id`,`target`,`type`) VALUES ('" + Session.GetHabbo().Id + "', @target, @type)");
                    dbClient.AddParameter("target", User);
                    dbClient.AddParameter("type", Type);
                    int newId = Convert.ToInt32(dbClient.InsertQuery());

                    if (!Session.GetHabbo().Relationships.ContainsKey(User))
                        Session.GetHabbo().Relationships.Add(User, new Relationship(newId, User, Type));
                }

                GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(User);
                if (Client != null)
                    Session.GetHabbo().GetMessenger().UpdateFriend(User, Client, true);
                else
                {
                    Habbo Habbo = PlusEnvironment.GetHabboById(User);
                    if (Habbo != null)
                    {
                        MessengerBuddy Buddy = null;
                        if (Session.GetHabbo().GetMessenger().TryGetFriend(User, out Buddy))
                            Session.SendPacket(new FriendListUpdateComposer(Session, Buddy));
                    }
                }
            }
        }
    }
}