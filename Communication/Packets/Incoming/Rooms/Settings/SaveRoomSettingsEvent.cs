using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Plus.Core;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Navigator;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class SaveRoomSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            string Name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            string Description = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            RoomAccess Access = RoomAccessUtility.ToRoomAccess(Packet.PopInt());
            string Password = Packet.PopString();
            int MaxUsers = Packet.PopInt();
            int CategoryId = Packet.PopInt();
            int TagCount = Packet.PopInt();

            List<string> Tags = new List<string>();
            StringBuilder formattedTags = new StringBuilder();

            for (int i = 0; i < TagCount; i++)
            {
                if (i > 0)
                {
                    formattedTags.Append(",");
                }

                string tag = Packet.PopString().ToLower();

                Tags.Add(tag);
                formattedTags.Append(tag);
            }

            int TradeSettings = Packet.PopInt();//2 = All can trade, 1 = owner only, 0 = no trading.
            int AllowPets = Convert.ToInt32(PlusEnvironment.BoolToEnum(Packet.PopBoolean()));
            int AllowPetsEat = Convert.ToInt32(PlusEnvironment.BoolToEnum(Packet.PopBoolean()));
            int RoomBlockingEnabled = Convert.ToInt32(PlusEnvironment.BoolToEnum(Packet.PopBoolean()));
            int Hidewall = Convert.ToInt32(PlusEnvironment.BoolToEnum(Packet.PopBoolean()));
            int WallThickness = Packet.PopInt();
            int FloorThickness = Packet.PopInt();
            int WhoMute = Packet.PopInt(); // mute
            int WhoKick = Packet.PopInt(); // kick
            int WhoBan = Packet.PopInt(); // ban

            int chatMode = Packet.PopInt();
            int chatSize = Packet.PopInt();
            int chatSpeed = Packet.PopInt();
            int chatDistance = Packet.PopInt();
            int extraFlood = Packet.PopInt();

            if (chatMode < 0 || chatMode > 1)
                chatMode = 0;

            if (chatSize < 0 || chatSize > 2)
                chatSize = 0;

            if (chatSpeed < 0 || chatSpeed > 2)
                chatSpeed = 0;

            if (chatDistance < 0)
                chatDistance = 1;

            if (chatDistance > 99)
                chatDistance = 100;

            if (extraFlood < 0 || extraFlood > 2)
                extraFlood = 0;

            if (TradeSettings < 0 || TradeSettings > 2)
                TradeSettings = 0;

            if (WhoMute < 0 || WhoMute > 1)
                WhoMute = 0;

            if (WhoKick < 0 || WhoKick > 1)
                WhoKick = 0;

            if (WhoBan < 0 || WhoBan > 1)
                WhoBan = 0;

            if (WallThickness < -2 || WallThickness > 1)
                WallThickness = 0;

            if (FloorThickness < -2 || FloorThickness > 1)
                FloorThickness = 0;

            if (Name.Length < 1)
                return;

            if (Name.Length > 60)
                Name = Name.Substring(0, 60);

            if (Access == RoomAccess.Password && Password.Length == 0)
                Access = RoomAccess.Open;

            if (MaxUsers < 0)
                MaxUsers = 10;

            if (MaxUsers > 50)
                MaxUsers = 50;

            SearchResultList SearchResultList = null;
            if (!PlusEnvironment.GetGame().GetNavigator().TryGetSearchResultList(CategoryId, out SearchResultList))
                CategoryId = 36;

            if (SearchResultList.CategoryType != NavigatorCategoryType.Category || SearchResultList.RequiredRank > Session.GetHabbo().Rank || (Session.GetHabbo().Id != Room.OwnerId && Session.GetHabbo().Rank >= SearchResultList.RequiredRank))
                CategoryId = 36;

            if (TagCount > 2)
                return;

            Room.AllowPets = AllowPets;
            Room.AllowPetsEating = AllowPetsEat;
            Room.RoomBlockingEnabled = RoomBlockingEnabled;
            Room.Hidewall = Hidewall;

            Room.RoomData.AllowPets = AllowPets;
            Room.RoomData.AllowPetsEating = AllowPetsEat;
            Room.RoomData.RoomBlockingEnabled = RoomBlockingEnabled;
            Room.RoomData.Hidewall = Hidewall;

            Room.Name = Name;
            Room.Access = Access;
            Room.Description = Description;
            Room.Category = CategoryId;
            Room.Password = Password;

            Room.RoomData.Name = Name;
            Room.RoomData.Access = Access;
            Room.RoomData.Description = Description;
            Room.RoomData.Category = CategoryId;
            Room.RoomData.Password = Password;

            Room.WhoCanBan = WhoBan;
            Room.WhoCanKick = WhoKick;
            Room.WhoCanMute = WhoMute;
            Room.RoomData.WhoCanBan = WhoBan;
            Room.RoomData.WhoCanKick = WhoKick;
            Room.RoomData.WhoCanMute = WhoMute;

            Room.ClearTags();
            Room.AddTagRange(Tags);
            Room.UsersMax = MaxUsers;

            Room.RoomData.Tags.Clear();
            Room.RoomData.Tags.AddRange(Tags);
            Room.RoomData.UsersMax = MaxUsers;

            Room.WallThickness = WallThickness;
            Room.FloorThickness = FloorThickness;
            Room.RoomData.WallThickness = WallThickness;
            Room.RoomData.FloorThickness = FloorThickness;

            Room.chatMode = chatMode;
            Room.chatSize = chatSize;
            Room.chatSpeed = chatSpeed;
            Room.chatDistance = chatDistance;
            Room.extraFlood = extraFlood;

            Room.TradeSettings = TradeSettings;

            Room.RoomData.chatMode = chatMode;
            Room.RoomData.chatSize = chatSize;
            Room.RoomData.chatSpeed = chatSpeed;
            Room.RoomData.chatDistance = chatDistance;
            Room.RoomData.extraFlood = extraFlood;

            Room.RoomData.TradeSettings = TradeSettings;

            string AccessStr = Password.Length > 0 ? "password" : "open";
            switch (Access)
            {
                default:
                case RoomAccess.Open:
                    AccessStr = "open";
                    break;

                case RoomAccess.Password:
                    AccessStr = "password";
                    break;

                case RoomAccess.Doorbell:
                    AccessStr = "locked";
                    break;

                case RoomAccess.Invisible:
                    AccessStr = "invisible";
                    break;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `rooms` SET `caption` = @caption, `description` = @description, `password` = @password, `category` = @categoryId, `state` = @state, `tags` = @tags, `users_max` = @maxUsers, `allow_pets` = @allowPets, `allow_pets_eat` = @allowPetsEat, `room_blocking_disabled` = @roomBlockingDisabled, `allow_hidewall` = @allowHidewall, `floorthick` = @floorThick, `wallthick` = @wallThick, `mute_settings` = @muteSettings, `kick_settings` = @kickSettings, `ban_settings` = @banSettings, `chat_mode` = @chatMode, `chat_size` = @chatSize, `chat_speed` = @chatSpeed, `chat_extra_flood` = @extraFlood, `chat_hearing_distance` = @chatDistance, `trade_settings` = @tradeSettings WHERE `id` = @roomId LIMIT 1");
                dbClient.AddParameter("categoryId", CategoryId);
                dbClient.AddParameter("maxUsers", MaxUsers);
                dbClient.AddParameter("allowPets", AllowPets);
                dbClient.AddParameter("allowPetsEat", AllowPetsEat);
                dbClient.AddParameter("roomBlockingDisabled", RoomBlockingEnabled);
                dbClient.AddParameter("allowHidewall", Room.Hidewall);
                dbClient.AddParameter("floorThick", Room.FloorThickness);
                dbClient.AddParameter("wallThick", Room.WallThickness);
                dbClient.AddParameter("muteSettings", Room.WhoCanMute);
                dbClient.AddParameter("kickSettings", Room.WhoCanKick);
                dbClient.AddParameter("banSettings", Room.WhoCanBan);
                dbClient.AddParameter("chatMode", Room.chatMode);
                dbClient.AddParameter("chatSize", Room.chatSize);
                dbClient.AddParameter("chatSpeed", Room.chatSpeed);
                dbClient.AddParameter("extraFlood", Room.extraFlood);
                dbClient.AddParameter("chatDistance", Room.chatDistance);
                dbClient.AddParameter("tradeSettings", Room.TradeSettings);
                dbClient.AddParameter("roomId", Room.Id);
                dbClient.AddParameter("caption", Room.Name);
                dbClient.AddParameter("description", Room.Description);
                dbClient.AddParameter("password", Room.Password);
                dbClient.AddParameter("state", AccessStr);
                dbClient.AddParameter("tags", formattedTags.ToString());
                dbClient.RunQuery();
            }

            Room.GetGameMap().GenerateMaps();

            if (Session.GetHabbo().CurrentRoom == null)
            {
                Session.SendPacket(new RoomSettingsSavedComposer(Room.RoomId));
                Session.SendPacket(new RoomInfoUpdatedComposer(Room.RoomId));
                Session.SendPacket(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, PlusEnvironment.EnumToBool(Room.Hidewall.ToString())));
            }
            else
            {
                Room.SendPacket(new RoomSettingsSavedComposer(Room.RoomId));
                Room.SendPacket(new RoomInfoUpdatedComposer(Room.RoomId));
                Room.SendPacket(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, PlusEnvironment.EnumToBool(Room.Hidewall.ToString())));
            }
            
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModDoorModeSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModWalkthroughSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModChatScrollSpeedSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModChatFloodFilterSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModChatHearRangeSeen", 1);
        }
    }
}
