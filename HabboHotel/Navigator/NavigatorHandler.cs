using Plus.Communication.Packets.Outgoing;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Plus.HabboHotel.Navigator
{
    static class NavigatorHandler
    {
        public static void Search(ServerPacket packet, SearchResultList result, string query, GameClient session, int limit)
        {
            if (session == null)
                return;
            
            switch (result.CategoryType)
            {
                default:
                case NavigatorCategoryType.MyFavourites:
                case NavigatorCategoryType.MyHistory:
                case NavigatorCategoryType.Featured:
                    packet.WriteInteger(0);
                    break;

                case NavigatorCategoryType.Query:
                    {
                        #region Query
                        if (query.ToLower().StartsWith("owner:"))
                        {
                            if (query.Length > 0)
                            {
                                int UserId = 0;
                                DataTable GetRooms = null;
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    if (query.ToLower().StartsWith("owner:"))
                                    {
                                        dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @username LIMIT 1");
                                        dbClient.AddParameter("username", query.Remove(0, 6));
                                        UserId = dbClient.GetInteger();

                                        dbClient.SetQuery("SELECT * FROM `rooms` WHERE `owner` = '" + UserId + "' and `state` != 'invisible' ORDER BY `users_now` DESC LIMIT 50");
                                        GetRooms = dbClient.GetTable();
                                    }
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (GetRooms != null)
                                {
                                    foreach (DataRow Row in GetRooms.Rows)
                                    {
                                        RoomData Data = null;
                                        if (!RoomFactory.TryGetData(Convert.ToInt32(Row["id"]), out Data))
                                            continue;

                                        if (!Results.Contains(Data))
                                            Results.Add(Data);
                                    }

                                    GetRooms = null;
                                }

                                packet.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                                }

                                Results = null;
                            }
                        }
                        else if (query.ToLower().StartsWith("tag:"))
                        {
                            query = query.Remove(0, 4);
                            ICollection<Room> TagMatches = PlusEnvironment.GetGame().GetRoomManager().SearchTaggedRooms(query);

                            packet.WriteInteger(TagMatches.Count);
                            foreach (RoomData Data in TagMatches.ToList())
                            {
                                RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                            }

                            TagMatches = null;
                        }
                        else if (query.ToLower().StartsWith("group:"))
                        {
                            query = query.Remove(0, 6);
                            ICollection<Room> GroupRooms = PlusEnvironment.GetGame().GetRoomManager().SearchGroupRooms(query);

                            packet.WriteInteger(GroupRooms.Count);
                            foreach (RoomData Data in GroupRooms.ToList())
                            {
                                RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                            }

                            GroupRooms = null;
                        }
                        else
                        {
                            if (query.Length > 0)
                            {
                                DataTable Table = null;
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT `id`,`caption`,`description`,`roomtype`,`owner`,`state`,`category`,`users_now`,`users_max`,`model_name`,`score`,`allow_pets`,`allow_pets_eat`,`room_blocking_disabled`,`allow_hidewall`,`password`,`wallpaper`,`floor`,`landscape`,`floorthick`,`wallthick`,`mute_settings`,`kick_settings`,`ban_settings`,`chat_mode`,`chat_speed`,`chat_size`,`trade_settings`,`group_id`,`tags`,`push_enabled`,`pull_enabled`,`enables_enabled`,`respect_notifications_enabled`,`pet_morphs_allowed`,`spush_enabled`,`spull_enabled`,`sale_price` FROM rooms WHERE `caption` LIKE @query ORDER BY `users_now` DESC LIMIT 50");
                                    dbClient.AddParameter("query", query + "%");
                                    Table = dbClient.GetTable();
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (Table != null)
                                {
                                    foreach (DataRow Row in Table.Rows)
                                    {
                                        if (Convert.ToString(Row["state"]) == "invisible")
                                            continue;

                                        RoomData Data = null;
                                        if (!RoomFactory.TryGetData(Convert.ToInt32(Row["id"]), out Data))
                                            continue;

                                        if (!Results.Contains(Data))
                                            Results.Add(Data);
                                    }

                                    Table = null;
                                }

                                packet.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                                }

                                Results = null;
                            }
                        }
                        #endregion

                        break;
                    }

                case NavigatorCategoryType.Popular:
                    {
                        List<Room> PopularRooms = PlusEnvironment.GetGame().GetRoomManager().GetPopularRooms(-1, limit);

                        packet.WriteInteger(PopularRooms.Count);
                        foreach (RoomData Data in PopularRooms.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        PopularRooms = null;
                        break;
                    }

                case NavigatorCategoryType.Recommended:
                    {
                        List<Room> RecommendedRooms = PlusEnvironment.GetGame().GetRoomManager().GetRecommendedRooms(limit);

                        packet.WriteInteger(RecommendedRooms.Count);
                        foreach (RoomData Data in RecommendedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        RecommendedRooms = null;
                        break;
                    }

                case NavigatorCategoryType.Category:
                    {
                        List<Room> GetRoomsByCategory = PlusEnvironment.GetGame().GetRoomManager().GetRoomsByCategory(result.Id, limit);

                        packet.WriteInteger(GetRoomsByCategory.Count);
                        foreach (RoomData Data in GetRoomsByCategory.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        GetRoomsByCategory = null;
                        break;
                    }

                case NavigatorCategoryType.MyRooms:
                    {
                        ICollection<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(session.GetHabbo().Id).OrderByDescending(x => x.UsersNow).ToList();

                        packet.WriteInteger(rooms.Count);
                        foreach (RoomData Data in rooms.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        break;
                    }

                case NavigatorCategoryType.MyFavourites:
                    {
                        break;
                    }

                case NavigatorCategoryType.MyGroups:
                    {
                        List<RoomData> MyGroups = new List<RoomData>();

                        foreach (Group Group in PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.GetHabbo().Id).ToList())
                        {
                            if (Group == null)
                                continue;

                            RoomData Data = null;
                            if (!RoomFactory.TryGetData(Group.RoomId, out Data))
                                continue;

                            if (!MyGroups.Contains(Data))
                                MyGroups.Add(Data);
                        }

                        MyGroups = MyGroups.Take(limit).ToList();

                        packet.WriteInteger(MyGroups.Count);
                        foreach (RoomData Data in MyGroups.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        MyGroups = null;

                        break;
                    }

                case NavigatorCategoryType.MyFriendsRooms:
                    {
                        List<int> RoomIds = new List<int>();

                        if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null || session.GetHabbo().GetMessenger().GetFriends() == null)
                            return;

                        foreach (MessengerBuddy buddy in session.GetHabbo().GetMessenger().GetFriends().Where(p => p.InRoom))
                        {
                            if (buddy == null || !buddy.InRoom || buddy.UserId == session.GetHabbo().Id)
                                continue;

                            if (!RoomIds.Contains(buddy.CurrentRoom.Id))
                                RoomIds.Add(buddy.CurrentRoom.Id);
                        }

                        List<Room> MyFriendsRooms = PlusEnvironment.GetGame().GetRoomManager().GetRoomsByIds(RoomIds.ToList());

                        packet.WriteInteger(MyFriendsRooms.Count);
                        foreach (RoomData Data in MyFriendsRooms.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        MyFriendsRooms = null;
                        break;
                    }

                case NavigatorCategoryType.MyRights:
                    {
                        List<RoomData> MyRights = new List<RoomData>();

                        if (session != null)
                        {
                            DataTable GetRights = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT `room_id` FROM `room_rights` WHERE `user_id` = @UserId LIMIT @FetchLimit");
                                dbClient.AddParameter("UserId", session.GetHabbo().Id);
                                dbClient.AddParameter("FetchLimit", limit);
                                GetRights = dbClient.GetTable();

                                foreach (DataRow Row in GetRights.Rows)
                                {
                                    RoomData Data = null;
                                    if (!RoomFactory.TryGetData(Convert.ToInt32(Row["room_id"]), out Data))
                                        continue;

                                    if (!MyRights.Contains(Data))
                                        MyRights.Add(Data);
                                }
                            }
                        }

                        packet.WriteInteger(MyRights.Count);
                        foreach (RoomData Data in MyRights.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        MyRights = null;
                        break;
                    }

                case NavigatorCategoryType.TopPromotions:
                    {
                        List<Room> GetPopularPromotions = PlusEnvironment.GetGame().GetRoomManager().GetOnGoingRoomPromotions(16, limit);

                        packet.WriteInteger(GetPopularPromotions.Count);
                        foreach (RoomData Data in GetPopularPromotions.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        GetPopularPromotions = null;

                        break;
                    }

                case NavigatorCategoryType.PromotionCategory:
                    {
                        List<Room> GetPromotedRooms = PlusEnvironment.GetGame().GetRoomManager().GetPromotedRooms(result.OrderId, limit);

                        packet.WriteInteger(GetPromotedRooms.Count);
                        foreach (RoomData Data in GetPromotedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(packet, Data, Data.Promotion);
                        }

                        GetPromotedRooms = null;

                        break;
                    }
            }
        }
    }
}
