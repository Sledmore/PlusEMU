using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;

using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.Communication.Packets.Outgoing;
using Plus.Utilities;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Cache;
using Plus.HabboHotel.Cache.Type;

namespace Plus.HabboHotel.Users.Messenger
{
    public class HabboMessenger
    {
        public bool AppearOffline;
        private readonly int _userId;

        private Dictionary<int, MessengerBuddy> _friends;
        private Dictionary<int, MessengerRequest> _requests;

        public HabboMessenger(int UserId)
        {
            this._userId = UserId;
            
            this._requests = new Dictionary<int, MessengerRequest>();
            this._friends = new Dictionary<int, MessengerBuddy>();
        }


        public void Init(Dictionary<int, MessengerBuddy> friends, Dictionary<int, MessengerRequest> requests)
        {
            this._requests = new Dictionary<int, MessengerRequest>(requests);
            this._friends = new Dictionary<int, MessengerBuddy>(friends);
        }

        public bool TryGetRequest(int senderID,  out MessengerRequest Request)
        {
            return this._requests.TryGetValue(senderID, out Request);
        }

        public bool TryGetFriend(int UserId, out MessengerBuddy Buddy)
        {
            return this._friends.TryGetValue(UserId, out Buddy);
        }

        public void ProcessOfflineMessages()
        {
            DataTable GetMessages = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `messenger_offline_messages` WHERE `to_id` = @id;");
                dbClient.AddParameter("id", this._userId);
                GetMessages = dbClient.GetTable();

                if (GetMessages != null)
                {
                    GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(this._userId);
                    if (Client == null)
                        return;

                    foreach (DataRow Row in GetMessages.Rows)
                    {
                        Client.SendPacket(new NewConsoleMessageComposer(Convert.ToInt32(Row["from_id"]), Convert.ToString(Row["message"]), (int)(UnixTimestamp.GetNow() - Convert.ToInt32(Row["timestamp"]))));
                    }

                    dbClient.SetQuery("DELETE FROM `messenger_offline_messages` WHERE `to_id` = @id");
                    dbClient.AddParameter("id", this._userId);
                    dbClient.RunQuery();
                }
            }
        }

        public void Destroy()
        {
            IEnumerable<GameClient> onlineUsers = PlusEnvironment.GetGame().GetClientManager().GetClientsById(_friends.Keys);

            foreach (GameClient client in onlineUsers)
            {
                if (client.GetHabbo() == null || client.GetHabbo().GetMessenger() == null)
                    continue;

                client.GetHabbo().GetMessenger().UpdateFriend(_userId, null, true);
            }
        }

        public void OnStatusChanged(bool notification)
        {
            if (GetClient() == null || GetClient().GetHabbo() == null || GetClient().GetHabbo().GetMessenger() == null)
                return;

            if (_friends == null)
                return;

            IEnumerable<GameClient> onlineUsers = PlusEnvironment.GetGame().GetClientManager().GetClientsById(_friends.Keys);
            if (onlineUsers.Count() == 0)
                return;

            foreach (GameClient client in onlineUsers.ToList())
            {
                try
                {
                    if (client == null || client.GetHabbo() == null || client.GetHabbo().GetMessenger() == null)
                        continue;

                    client.GetHabbo().GetMessenger().UpdateFriend(_userId, client, true);

                    if (this == null || client == null || client.GetHabbo() == null)
                        continue;

                    UpdateFriend(client.GetHabbo().Id, client, notification);
                }
                catch
                {
                    continue;
                }
            }
        }

        public void UpdateFriend(int userid, GameClient client, bool notification)
        {
            if (_friends.ContainsKey(userid))
            {
                _friends[userid].UpdateUser(client);

                if (notification)
                {
                    GameClient Userclient = GetClient();
                    if (Userclient != null)
                        Userclient.SendPacket(SerializeUpdate(_friends[userid]));
                }
            }
        }

        public void HandleAllRequests()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM messenger_requests WHERE from_id = " + _userId + " OR to_id = " + _userId);
            }

            ClearRequests();
        }

        public void HandleRequest(int sender)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM messenger_requests WHERE (from_id = " + _userId + " AND to_id = " +       sender + ") OR (to_id = " + _userId + " AND from_id = " + sender + ")");
            }

            _requests.Remove(sender);
        }

        public void CreateFriendship(int friendID)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("REPLACE INTO messenger_friendships (user_one_id,user_two_id) VALUES (" + _userId + "," + friendID + ")");
            }

            OnNewFriendship(friendID);

            GameClient User = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);

            if (User != null && User.GetHabbo().GetMessenger() != null)
            {
                User.GetHabbo().GetMessenger().OnNewFriendship(_userId);
            }

            if (User != null)
                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User, "ACH_FriendListSize", 1);

            GameClient thisUser = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(_userId);
            if (thisUser != null)
                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(thisUser, "ACH_FriendListSize", 1);
        }

        public void DestroyFriendship(int friendID)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM messenger_friendships WHERE (user_one_id = " + _userId +     " AND user_two_id = " + friendID + ") OR (user_two_id = " + _userId +  " AND user_one_id = " + friendID + ")");

            }

            OnDestroyFriendship(friendID);

            GameClient User = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);

            if (User != null && User.GetHabbo().GetMessenger() != null)
                User.GetHabbo().GetMessenger().OnDestroyFriendship(_userId);
        }

        public void OnNewFriendship(int friendID)
        {
            GameClient friend = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);

            MessengerBuddy newFriend;
            if (friend == null || friend.GetHabbo() == null)
            {
                DataRow dRow;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT id,username,motto,look,last_online,hide_inroom,hide_online FROM users WHERE `id` = @friendid LIMIT 1");
                    dbClient.AddParameter("friendid", friendID);
                    dRow = dbClient.GetRow();
                }

                newFriend = new MessengerBuddy(friendID, Convert.ToString(dRow["username"]), Convert.ToString(dRow["look"]), Convert.ToString(dRow["motto"]), Convert.ToInt32(dRow["last_online"]),
                    PlusEnvironment.EnumToBool(dRow["hide_online"].ToString()), PlusEnvironment.EnumToBool(dRow["hide_inroom"].ToString()));
            }
            else
            {
                Habbo user = friend.GetHabbo();


                newFriend = new MessengerBuddy(friendID, user.Username, user.Look, user.Motto, 0, user.AppearOffline, user.AllowPublicRoomStatus);
                newFriend.UpdateUser(friend);
            }

            if (!_friends.ContainsKey(friendID))
                _friends.Add(friendID, newFriend);

            GetClient().SendPacket(SerializeUpdate(newFriend));
        }

        public bool RequestExists(int requestID)
        {
            if (_requests.ContainsKey(requestID))
                return true;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery(
                    "SELECT user_one_id FROM messenger_friendships WHERE user_one_id = @myID AND user_two_id = @friendID");
                dbClient.AddParameter("myID", Convert.ToInt32(_userId));
                dbClient.AddParameter("friendID", Convert.ToInt32(requestID));
                return dbClient.FindsResult();
            }
        }

        public bool FriendshipExists(int friendID)
        {
            return _friends.ContainsKey(friendID);
        }

        public void OnDestroyFriendship(int Friend)
        {
            if (_friends.ContainsKey(Friend))
                _friends.Remove(Friend);

            GetClient().SendPacket(new FriendListUpdateComposer(Friend));
        }

        public bool RequestBuddy(string UserQuery)
        {
            int userID;
            bool hasFQDisabled;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(UserQuery);
            if (client == null)
            {
                DataRow Row = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id`,`block_newfriends` FROM `users` WHERE `username` = @query LIMIT 1");
                    dbClient.AddParameter("query", UserQuery.ToLower());
                    Row = dbClient.GetRow();
                }

                if (Row == null)
                    return false;

                userID = Convert.ToInt32(Row["id"]);
                hasFQDisabled = PlusEnvironment.EnumToBool(Row["block_newfriends"].ToString());
            }
            else
            {
                userID = client.GetHabbo().Id;
                hasFQDisabled = client.GetHabbo().AllowFriendRequests;
            }

            if (hasFQDisabled)
            {
                GetClient().SendPacket(new MessengerErrorComposer(39, 3));
                return false;
            }

            int ToId = userID;
            if (RequestExists(ToId))
                return true;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("REPLACE INTO `messenger_requests` (`from_id`,`to_id`) VALUES ('" + _userId + "','" + ToId + "')");
            }

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(GetClient(), QuestType.ADD_FRIENDS);

            GameClient ToUser = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(ToId);
            if (ToUser == null || ToUser.GetHabbo() == null)
                return true;

            MessengerRequest Request = new MessengerRequest(ToId, _userId, PlusEnvironment.GetGame().GetClientManager().GetNameById(_userId));

            ToUser.GetHabbo().GetMessenger().OnNewRequest(_userId);

            UserCache ThisUser = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(_userId);

            if (ThisUser != null)
                ToUser.SendPacket(new NewBuddyRequestComposer(ThisUser));

            _requests.Add(ToId, Request);
            return true;
        }

        public void OnNewRequest(int friendID)
        {
            if (!_requests.ContainsKey(friendID))
                _requests.Add(friendID, new MessengerRequest(_userId, friendID, PlusEnvironment.GetGame().GetClientManager().GetNameById(friendID)));
        }

        public void SendInstantMessage(int ToId, string Message)
        {
            if (ToId == 0)
                return;

            if (GetClient() == null)
                return;

            if (GetClient().GetHabbo() == null)
                return;

            if (!FriendshipExists(ToId))
            {
                GetClient().SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.YOUR_NOT_FRIENDS, ToId));
                return;
            }

            if (GetClient().GetHabbo().MessengerSpamCount >= 12)
            {
                GetClient().GetHabbo().MessengerSpamTime = PlusEnvironment.GetUnixTimestamp() + 60;
                GetClient().GetHabbo().MessengerSpamCount = 0;
                GetClient().SendNotification("You cannot send a message, you have flooded the console.\n\nYou can send a message in 60 seconds.");
                return;
            }
            else if (GetClient().GetHabbo().MessengerSpamTime > PlusEnvironment.GetUnixTimestamp())
            {
                double Time = GetClient().GetHabbo().MessengerSpamTime - PlusEnvironment.GetUnixTimestamp();
                GetClient().SendNotification("You cannot send a message, you have flooded the console.\n\nYou can send a message in " + Time + " seconds.");
                return;
            }


            GetClient().GetHabbo().MessengerSpamCount++;

            GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(ToId);
            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetMessenger() == null)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `messenger_offline_messages` (`to_id`, `from_id`, `message`, `timestamp`) VALUES (@tid, @fid, @msg, UNIX_TIMESTAMP())");
                    dbClient.AddParameter("tid", ToId);
                    dbClient.AddParameter("fid", GetClient().GetHabbo().Id);
                    dbClient.AddParameter("msg", Message);
                    dbClient.RunQuery();
                }
                return;
            }

            if (!Client.GetHabbo().AllowConsoleMessages || Client.GetHabbo().GetIgnores().IgnoredUserIds().Contains(GetClient().GetHabbo().Id))
            {
                GetClient().SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.FRIEND_BUSY, ToId));
                return;
            }

            if (GetClient().GetHabbo().TimeMuted > 0)
            {
                GetClient().SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.YOUR_MUTED, ToId));
                return;
            }

            if (Client.GetHabbo().TimeMuted > 0)
            {
                GetClient().SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.FRIEND_MUTED, ToId));
            }

            if (String.IsNullOrEmpty(Message))
                return;

            Client.SendPacket(new NewConsoleMessageComposer(_userId, Message));
            LogPM(_userId, ToId, Message);
        }

        public void LogPM(int From_Id, int ToId, string Message)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO chatlogs_console VALUES (NULL, " + From_Id + ", " + ToId + ", @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }
        }

        public ServerPacket SerializeUpdate(MessengerBuddy friend)
        {
            ServerPacket Packet = new ServerPacket(ServerPacketHeader.FriendListUpdateMessageComposer);
            Packet.WriteInteger(0); // category count
            Packet.WriteInteger(1); // number of updates
            Packet.WriteInteger(0); // don't know

            friend.Serialize(Packet, GetClient());
            return Packet;
        }

        public void BroadcastAchievement(int UserId, MessengerEventTypes Type, string Data)
        {
            IEnumerable<GameClient> MyFriends = PlusEnvironment.GetGame().GetClientManager().GetClientsById(this._friends.Keys);

            foreach (GameClient Client in MyFriends.ToList())
            {
                if (Client.GetHabbo() != null && Client.GetHabbo().GetMessenger() != null)
                {
                    Client.SendPacket(new FriendNotificationComposer(UserId, Type, Data));
                    Client.GetHabbo().GetMessenger().OnStatusChanged(true);
                }
            }
        }

        public void ClearRequests()
        {
          this.  _requests.Clear();
        }

        private GameClient GetClient()
        {
            return PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(this._userId);
        }

        public ICollection<MessengerRequest> GetRequests()
        {
            return this._requests.Values;
        }

        public ICollection<MessengerBuddy> GetFriends()
        {
            return this._friends.Values;
        }
    }
}