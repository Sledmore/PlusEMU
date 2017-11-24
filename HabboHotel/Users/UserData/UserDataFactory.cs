using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users.Authenticator;
using Plus.HabboHotel.Users.Badges;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Relationships;

namespace Plus.HabboHotel.Users.UserData
{
    public class UserDataFactory
    {
        public static UserData GetUserData(string SessionTicket, out byte errorCode)
        {
            int UserId;
            DataRow dUserInfo = null;
            DataTable dAchievements = null;
            DataTable dFavouriteRooms = null;
            DataTable dBadges = null;
            DataTable dFriends = null;
            DataTable dRequests = null;
            DataTable dRooms = null;
            DataTable dQuests = null;
            DataTable dRelations = null;
            DataRow UserInfo = null;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`rank`,`motto`,`look`,`gender`,`last_online`,`credits`,`activity_points`,`home_room`,`block_newfriends`,`hide_online`,`hide_inroom`,`vip`,`account_created`,`vip_points`,`machine_id`,`volume`,`chat_preference`,`focus_preference`, `pets_muted`,`bots_muted`,`advertising_report_blocked`,`last_change`,`gotw_points`,`ignore_invites`,`time_muted`,`allow_gifts`,`friend_bar_state`,`disable_forced_effects`,`allow_mimic`,`rank_vip` FROM `users` WHERE `auth_ticket` = @sso LIMIT 1");
                dbClient.AddParameter("sso", SessionTicket);
                dUserInfo = dbClient.GetRow();

                if (dUserInfo == null)
                {
                    errorCode = 1;
                    return null;
                }

                UserId = Convert.ToInt32(dUserInfo["id"]);
                if (PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId) != null)
                {
                    errorCode = 2;
                    PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId).Disconnect();
                    return null;
                }

                dbClient.SetQuery("SELECT `group`,`level`,`progress` FROM `user_achievements` WHERE `userid` = '" + UserId + "'");
                dAchievements = dbClient.GetTable();

                dbClient.SetQuery("SELECT room_id FROM user_favorites WHERE `user_id` = '" + UserId + "'");
                dFavouriteRooms = dbClient.GetTable();
                
                dbClient.SetQuery("SELECT `badge_id`,`badge_slot` FROM user_badges WHERE `user_id` = '" + UserId + "'");
                dBadges = dbClient.GetTable();

                dbClient.SetQuery(
                    "SELECT users.id,users.username,users.motto,users.look,users.last_online,users.hide_inroom,users.hide_online " +
                    "FROM users " +
                    "JOIN messenger_friendships " +
                    "ON users.id = messenger_friendships.user_one_id " +
                    "WHERE messenger_friendships.user_two_id = " + UserId + " " +
                    "UNION ALL " +
                    "SELECT users.id,users.username,users.motto,users.look,users.last_online,users.hide_inroom,users.hide_online " +
                    "FROM users " +
                    "JOIN messenger_friendships " +
                    "ON users.id = messenger_friendships.user_two_id " +
                    "WHERE messenger_friendships.user_one_id = " + UserId);
                dFriends = dbClient.GetTable();

                dbClient.SetQuery("SELECT messenger_requests.from_id,messenger_requests.to_id,users.username FROM users JOIN messenger_requests ON users.id = messenger_requests.from_id WHERE messenger_requests.to_id = " + UserId);
                dRequests = dbClient.GetTable();

                dbClient.SetQuery("SELECT * FROM rooms WHERE `owner` = '" + UserId + "' LIMIT 150");
                dRooms = dbClient.GetTable();

                dbClient.SetQuery("SELECT `quest_id`,`progress` FROM user_quests WHERE `user_id` = '" + UserId + "'");
                dQuests = dbClient.GetTable();

                dbClient.SetQuery("SELECT `id`,`user_id`,`target`,`type` FROM `user_relationships` WHERE `user_id` = '" + UserId + "'");
                dRelations = dbClient.GetTable();

                dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                UserInfo = dbClient.GetRow();
                if (UserInfo == null)
                {
                    dbClient.RunQuery("INSERT INTO `user_info` (`user_id`) VALUES ('" + UserId + "')");

                    dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                    UserInfo = dbClient.GetRow();
                }

                dbClient.RunQuery("UPDATE `users` SET `online` = '1', `auth_ticket` = '' WHERE `id` = '" + UserId + "' LIMIT 1");
            }

            ConcurrentDictionary<string, UserAchievement> Achievements = new ConcurrentDictionary<string, UserAchievement>();
            foreach (DataRow dRow in dAchievements.Rows)
            {
                Achievements.TryAdd(Convert.ToString(dRow["group"]), new UserAchievement(Convert.ToString(dRow["group"]), Convert.ToInt32(dRow["level"]), Convert.ToInt32(dRow["progress"])));
            }

            List<int> favouritedRooms = new List<int>();
            foreach (DataRow dRow in dFavouriteRooms.Rows)
            {
                favouritedRooms.Add(Convert.ToInt32(dRow["room_id"]));
            }

            List<Badge> badges = new List<Badge>();
            foreach (DataRow dRow in dBadges.Rows)
            {
                badges.Add(new Badge(Convert.ToString(dRow["badge_id"]), Convert.ToInt32(dRow["badge_slot"])));
            }

            Dictionary<int, MessengerBuddy> friends = new Dictionary<int, MessengerBuddy>();
            foreach (DataRow dRow in dFriends.Rows)
            {
                int friendID = Convert.ToInt32(dRow["id"]);
                string friendName = Convert.ToString(dRow["username"]);
                string friendLook = Convert.ToString(dRow["look"]);
                string friendMotto = Convert.ToString(dRow["motto"]);
                int friendLastOnline = Convert.ToInt32(dRow["last_online"]);
                bool friendHideOnline = PlusEnvironment.EnumToBool(dRow["hide_online"].ToString());
                bool friendHideRoom = PlusEnvironment.EnumToBool(dRow["hide_inroom"].ToString());

                if (friendID == UserId)
                    continue;

                if (!friends.ContainsKey(friendID))
                    friends.Add(friendID, new MessengerBuddy(friendID, friendName, friendLook, friendMotto, friendLastOnline, friendHideOnline, friendHideRoom));
            }

            Dictionary<int, MessengerRequest> requests = new Dictionary<int, MessengerRequest>();
            foreach (DataRow dRow in dRequests.Rows)
            {
                int receiverID = Convert.ToInt32(dRow["from_id"]);
                int senderID = Convert.ToInt32(dRow["to_id"]);

                string requestUsername = Convert.ToString(dRow["username"]);

                if (receiverID != UserId)
                {
                    if (!requests.ContainsKey(receiverID))
                        requests.Add(receiverID, new MessengerRequest(UserId, receiverID, requestUsername));
                }
                else
                {
                    if (!requests.ContainsKey(senderID))
                        requests.Add(senderID, new MessengerRequest(UserId, senderID, requestUsername));
                }
            }

            List<RoomData> rooms = new List<RoomData>();
            foreach (DataRow dRow in dRooms.Rows)
            {
                rooms.Add(PlusEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(dRow["id"]), dRow));
            }

            Dictionary<int, int> quests = new Dictionary<int, int>();
            foreach (DataRow dRow in dQuests.Rows)
            {
                int questId = Convert.ToInt32(dRow["quest_id"]);

                if (quests.ContainsKey(questId))
                    quests.Remove(questId);

                quests.Add(questId, Convert.ToInt32(dRow["progress"]));
            }

            Dictionary<int, Relationship> Relationships = new Dictionary<int, Relationship>();
            foreach (DataRow Row in dRelations.Rows)
            {
                if (friends.ContainsKey(Convert.ToInt32(Row[2])))
                    Relationships.Add(Convert.ToInt32(Row[2]), new Relationship(Convert.ToInt32(Row[0]), Convert.ToInt32(Row[2]), Convert.ToInt32(Row[3].ToString())));
            }

            Habbo user = HabboFactory.GenerateHabbo(dUserInfo, UserInfo);

            dUserInfo = null;
            dAchievements = null;
            dFavouriteRooms = null;
            dBadges = null;
            dFriends = null;
            dRequests = null;
            dRooms = null;
            dRelations = null;

            errorCode = 0;
            return new UserData(UserId, Achievements, favouritedRooms, badges, friends, requests, rooms, quests, user, Relationships);
        }

        public static UserData GetUserData(int UserId)
        {
            DataRow dUserInfo = null;
            DataRow UserInfo = null;
            DataTable dRelations = null;
            DataTable dGroups = null;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`rank`,`motto`,`look`,`gender`,`last_online`,`credits`,`activity_points`,`home_room`,`block_newfriends`,`hide_online`,`hide_inroom`,`vip`,`account_created`,`vip_points`,`machine_id`,`volume`,`chat_preference`, `focus_preference`, `pets_muted`,`bots_muted`,`advertising_report_blocked`,`last_change`,`gotw_points`,`ignore_invites`,`time_muted`,`allow_gifts`,`friend_bar_state`,`disable_forced_effects`,`allow_mimic`,`rank_vip` FROM `users` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", UserId);
                dUserInfo = dbClient.GetRow();

                PlusEnvironment.GetGame().GetClientManager().LogClonesOut(Convert.ToInt32(UserId));

                if (dUserInfo == null)
                    return null;

                if (PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId) != null)
                    return null;


                dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                UserInfo = dbClient.GetRow();
                if (UserInfo == null)
                {
                    dbClient.RunQuery("INSERT INTO `user_info` (`user_id`) VALUES ('" + UserId + "')");

                    dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                    UserInfo = dbClient.GetRow();
                }

                dbClient.SetQuery("SELECT group_id,rank FROM group_memberships WHERE user_id=@id");
                dbClient.AddParameter("id", UserId);
                dGroups = dbClient.GetTable();

                dbClient.SetQuery("SELECT `id`,`target`,`type` FROM user_relationships WHERE user_id=@id");
                dbClient.AddParameter("id", UserId);
                dRelations = dbClient.GetTable();
            }

            ConcurrentDictionary<string, UserAchievement> Achievements = new ConcurrentDictionary<string, UserAchievement>();
            List<int> FavouritedRooms = new List<int>();
            List<Badge> Badges = new List<Badge>();
            Dictionary<int, MessengerBuddy> Friends = new Dictionary<int, MessengerBuddy>();
            Dictionary<int, MessengerRequest> FriendRequests = new Dictionary<int, MessengerRequest>();
            List<RoomData> Rooms = new List<RoomData>();
            Dictionary<int, int> Quests = new Dictionary<int, int>();

            Dictionary<int, Relationship> Relationships = new Dictionary<int, Relationship>();
            foreach (DataRow Row in dRelations.Rows)
            {
                if (!Relationships.ContainsKey(Convert.ToInt32(Row["id"])))
                {
                    Relationships.Add(Convert.ToInt32(Row["target"]), new Relationship(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["target"]), Convert.ToInt32(Row["type"].ToString())));
                }
            }

            Habbo user = HabboFactory.GenerateHabbo(dUserInfo, UserInfo);
            return new UserData(UserId, Achievements, FavouritedRooms, Badges, Friends, FriendRequests, Rooms, Quests, user, Relationships);
        }
    }
}