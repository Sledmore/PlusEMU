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
        public static void Search(ServerPacket Message, SearchResultList SearchResult, string SearchData, GameClient Session, int FetchLimit)
        {
            //Switching by categorys.
            switch (SearchResult.CategoryType)
            {
                default:
                    Message.WriteInteger(0);
                    break;

                case NavigatorCategoryType.Query:
                    {
                        #region Query
                        if (SearchData.ToLower().StartsWith("owner:"))
                        {
                            if (SearchData.Length > 0)
                            {
                                int UserId = 0;
                                DataTable GetRooms = null;
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    if (SearchData.ToLower().StartsWith("owner:"))
                                    {
                                        dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @username LIMIT 1");
                                        dbClient.AddParameter("username", SearchData.Remove(0, 6));
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
                                        RoomData RoomData = PlusEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                        if (RoomData != null && !Results.Contains(RoomData))
                                            Results.Add(RoomData);
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                                }
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("tag:"))
                        {
                            SearchData = SearchData.Remove(0, 4);
                            ICollection<RoomData> TagMatches = PlusEnvironment.GetGame().GetRoomManager().SearchTaggedRooms(SearchData);

                            Message.WriteInteger(TagMatches.Count);
                            foreach (RoomData Data in TagMatches.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("group:"))
                        {
                            SearchData = SearchData.Remove(0, 6);
                            ICollection<RoomData> GroupRooms = PlusEnvironment.GetGame().GetRoomManager().SearchGroupRooms(SearchData);

                            Message.WriteInteger(GroupRooms.Count);
                            foreach (RoomData Data in GroupRooms.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                            }
                        }
                        else
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable Table = null;
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT `id`,`caption`,`description`,`roomtype`,`owner`,`state`,`category`,`users_now`,`users_max`,`model_name`,`score`,`allow_pets`,`allow_pets_eat`,`room_blocking_disabled`,`allow_hidewall`,`password`,`wallpaper`,`floor`,`landscape`,`floorthick`,`wallthick`,`mute_settings`,`kick_settings`,`ban_settings`,`chat_mode`,`chat_speed`,`chat_size`,`trade_settings`,`group_id`,`tags`,`push_enabled`,`pull_enabled`,`enables_enabled`,`respect_notifications_enabled`,`pet_morphs_allowed`,`spush_enabled`,`spull_enabled` FROM rooms WHERE `caption` LIKE @query ORDER BY `users_now` DESC LIMIT 50");
                                    dbClient.AddParameter("query", "%" + SearchData + "%");
                                    Table = dbClient.GetTable();
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (Table != null)
                                {
                                    foreach (DataRow Row in Table.Rows)
                                    {
                                        if (Convert.ToString(Row["state"]) == "invisible")
                                            continue;

                                        RoomData RData = PlusEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                        if (RData != null && !Results.Contains(RData))
                                            Results.Add(RData);
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                                }
                            }
                        }
                        #endregion

                        break;
                    }

                case NavigatorCategoryType.Featured:
                    #region Featured
                    List<RoomData> Rooms = new List<RoomData>();
                    ICollection<FeaturedRoom> Featured = PlusEnvironment.GetGame().GetNavigator().GetFeaturedRooms();
                    foreach (FeaturedRoom FeaturedItem in Featured.ToList())
                    {
                        if (FeaturedItem == null)
                            continue;

                        RoomData Data = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(FeaturedItem.RoomId);
                        if (Data == null)
                            continue;

                        if (!Rooms.Contains(Data))
                            Rooms.Add(Data);
                    }

                    Message.WriteInteger(Rooms.Count);
                    foreach (RoomData Data in Rooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    #endregion
                    break;

                case NavigatorCategoryType.Popular:
                    {
                        List<RoomData> PopularRooms = PlusEnvironment.GetGame().GetRoomManager().GetPopularRooms(-1, FetchLimit);

                        Message.WriteInteger(PopularRooms.Count);
                        foreach (RoomData Data in PopularRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.Recommended:
                    {
                        List<RoomData> RecommendedRooms = PlusEnvironment.GetGame().GetRoomManager().GetRecommendedRooms(FetchLimit);

                        Message.WriteInteger(RecommendedRooms.Count);
                        foreach (RoomData Data in RecommendedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.Category:
                    {
                        List<RoomData> GetRoomsByCategory = PlusEnvironment.GetGame().GetRoomManager().GetRoomsByCategory(SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetRoomsByCategory.Count);
                        foreach (RoomData Data in GetRoomsByCategory.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.MyRooms:

                    Message.WriteInteger(Session.GetHabbo().UsersRooms.Count);
                    foreach (RoomData Data in Session.GetHabbo().UsersRooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MyFavourites:
                    List<RoomData> Favourites = new List<RoomData>();
                    foreach (int Id in Session.GetHabbo().FavoriteRooms.ToArray())
                    {
                        RoomData Room = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);
                        if (Room == null)
                            continue;

                        if (!Favourites.Contains(Room))
                            Favourites.Add(Room);
                    }

                    Favourites = Favourites.Take(FetchLimit).ToList();

                    Message.WriteInteger(Favourites.Count);
                    foreach (RoomData Data in Favourites.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MyGroups:
                    List<RoomData> MyGroups = new List<RoomData>();

                    foreach (Group Group in PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id).ToList())
                    {
                        if (Group == null)
                            continue;

                        RoomData Data = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId);
                        if (Data == null)
                            continue;

                        if (!MyGroups.Contains(Data))
                            MyGroups.Add(Data);
                    }

                    MyGroups = MyGroups.Take(FetchLimit).ToList();

                    Message.WriteInteger(MyGroups.Count);
                    foreach (RoomData Data in MyGroups.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MyFriendsRooms:
                    List<RoomData> MyFriendsRooms = new List<RoomData>();
                    foreach (MessengerBuddy buddy in Session.GetHabbo().GetMessenger().GetFriends().Where(p => p.InRoom))
                    {
                        if (buddy == null || !buddy.InRoom || buddy.UserId == Session.GetHabbo().Id)
                            continue;

                        if (!MyFriendsRooms.Contains(buddy.CurrentRoom.RoomData))
                            MyFriendsRooms.Add(buddy.CurrentRoom.RoomData);
                    }

                    Message.WriteInteger(MyFriendsRooms.Count);
                    foreach (RoomData Data in MyFriendsRooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MyRights:
                    List<RoomData> MyRights = new List<RoomData>();

                    DataTable GetRights = null;
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT `room_id` FROM `room_rights` WHERE `user_id` = @UserId LIMIT @FetchLimit");
                        dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                        dbClient.AddParameter("FetchLimit", FetchLimit);
                        GetRights = dbClient.GetTable();

                        foreach (DataRow Row in GetRights.Rows)
                        {
                            RoomData Data = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(Convert.ToInt32(Row["room_id"]));
                            if (Data == null)
                                continue;

                            if (!MyRights.Contains(Data))
                                MyRights.Add(Data);
                        }
                    }

                    Message.WriteInteger(MyRights.Count);
                    foreach (RoomData Data in MyRights.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.TopPromotions:
                    {
                        List<RoomData> GetPopularPromotions = PlusEnvironment.GetGame().GetRoomManager().GetOnGoingRoomPromotions(16, FetchLimit);

                        Message.WriteInteger(GetPopularPromotions.Count);
                        foreach (RoomData Data in GetPopularPromotions.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.PromotionCategory:
                    {
                        List<RoomData> GetPromotedRooms = PlusEnvironment.GetGame().GetRoomManager().GetPromotedRooms(SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetPromotedRooms.Count);
                        foreach (RoomData Data in GetPromotedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }
            }
        }
    }
}