using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Users
{
    class OpenPlayerProfileEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int userID = Packet.PopInt();
            Boolean IsMe = Packet.PopBoolean();

            Habbo targetData = PlusEnvironment.GetHabboById(userID);
            if (targetData == null)
            {
                Session.SendNotification("An error occured whilst finding that user's profile.");
                return;
            }
            
            List<Group> Groups = PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(targetData.Id);
            
            int friendCount = 0;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `messenger_friendships` WHERE (`user_one_id` = @userid OR `user_two_id` = @userid)");
                dbClient.AddParameter("userid", userID);
                friendCount = dbClient.GetInteger();
            }

            Session.SendPacket(new ProfileInformationComposer(targetData, Session, Groups, friendCount));
        }
    }
}
