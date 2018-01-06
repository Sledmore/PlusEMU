using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Messenger
{
    class RemoveBuddyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int Amount = Packet.PopInt();
            if (Amount > 100)
                Amount = 100;
            else if (Amount < 0)
                return;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                for (int i = 0; i < Amount; i++)
                {
                    int Id = Packet.PopInt();

                    if (Session.GetHabbo().Relationships.Count(x => x.Value.UserId == Id) > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = @id AND `target` = @target OR `target` = @id AND `user_id` = @target");
                        dbClient.AddParameter("id", Session.GetHabbo().Id);
                        dbClient.AddParameter("target", Id);
                        dbClient.RunQuery();
                    }

                    if (Session.GetHabbo().Relationships.ContainsKey(Id))
                        Session.GetHabbo().Relationships.Remove(Id);

                    GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Id);
                    if (Target != null)
                    {
                        if (Target.GetHabbo().Relationships.ContainsKey(Session.GetHabbo().Id))
                            Target.GetHabbo().Relationships.Remove(Session.GetHabbo().Id);
                    }

                    Session.GetHabbo().GetMessenger().DestroyFriendship(Id);
                }
            }
        }
    }
}