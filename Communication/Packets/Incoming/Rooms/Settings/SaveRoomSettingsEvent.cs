using System;
using System.Text;
using System.Collections.Generic;
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
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            int roomId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetRoomManager().TryLoadRoom(roomId, out Room room))
                return;

            string Name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string Description = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            RoomAccess Access = RoomAccessUtility.ToRoomAccess(packet.PopInt());
            string Password = packet.PopString();
            int MaxUsers = packet.PopInt();
            int CategoryId = packet.PopInt();
            int TagCount = packet.PopInt();

            List<string> Tags = new List<string>();
            StringBuilder formattedTags = new StringBuilder();

            for (int i = 0; i < TagCount; i++)
            {
                if (i > 0)
                {
                    formattedTags.Append(",");
                }

                string tag = packet.PopString().ToLower();

                Tags.Add(tag);
                formattedTags.Append(tag);
            }

            int TradeSettings = packet.PopInt();//2 = All can trade, 1 = owner only, 0 = no trading.
            int AllowPets = Convert.ToInt32(PlusEnvironment.BoolToEnum(packet.PopBoolean()));
            int AllowPetsEat = Convert.ToInt32(PlusEnvironment.BoolToEnum(packet.PopBoolean()));
            int RoomBlockingEnabled = Convert.ToInt32(PlusEnvironment.BoolToEnum(packet.PopBoolean()));
            int Hidewall = Convert.ToInt32(PlusEnvironment.BoolToEnum(packet.PopBoolean()));
            int WallThickness = packet.PopInt();
            int FloorThickness = packet.PopInt();
            int WhoMute = packet.PopInt(); // mute
            int WhoKick = packet.PopInt(); // kick
            int WhoBan = packet.PopInt(); // ban

            int chatMode = packet.PopInt();
            int chatSize = packet.PopInt();
            int chatSpeed = packet.PopInt();
            int chatDistance = packet.PopInt();
            int extraFlood = packet.PopInt();

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

            if (SearchResultList.CategoryType != NavigatorCategoryType.Category || SearchResultList.RequiredRank > session.GetHabbo().Rank || (session.GetHabbo().Id != room.OwnerId && session.GetHabbo().Rank >= SearchResultList.RequiredRank))
                CategoryId = 36;

            if (TagCount > 2)
                return;

            room.AllowPets = AllowPets;
            room.AllowPetsEating = AllowPetsEat;
            room.RoomBlockingEnabled = RoomBlockingEnabled;
            room.Hidewall = Hidewall;

            room.Name = Name;
            room.Access = Access;
            room.Description = Description;
            room.Category = CategoryId;
            room.Password = Password;

            room.WhoCanBan = WhoBan;
            room.WhoCanKick = WhoKick;
            room.WhoCanMute = WhoMute;

            room.ClearTags();
            room.AddTagRange(Tags);
            room.UsersMax = MaxUsers;

            room.WallThickness = WallThickness;
            room.FloorThickness = FloorThickness;

            room.chatMode = chatMode;
            room.ChatSize = chatSize;
            room.ChatSpeed = chatSpeed;
            room.ChatDistance = chatDistance;
            room.ExtraFlood = extraFlood;

            room.TradeSettings = TradeSettings;

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
                dbClient.AddParameter("allowHidewall", room.Hidewall);
                dbClient.AddParameter("floorThick", room.FloorThickness);
                dbClient.AddParameter("wallThick", room.WallThickness);
                dbClient.AddParameter("muteSettings", room.WhoCanMute);
                dbClient.AddParameter("kickSettings", room.WhoCanKick);
                dbClient.AddParameter("banSettings", room.WhoCanBan);
                dbClient.AddParameter("chatMode", room.chatMode);
                dbClient.AddParameter("chatSize", room.ChatSize);
                dbClient.AddParameter("chatSpeed", room.ChatSpeed);
                dbClient.AddParameter("extraFlood", room.ExtraFlood);
                dbClient.AddParameter("chatDistance", room.ChatDistance);
                dbClient.AddParameter("tradeSettings", room.TradeSettings);
                dbClient.AddParameter("roomId", room.Id);
                dbClient.AddParameter("caption", room.Name);
                dbClient.AddParameter("description", room.Description);
                dbClient.AddParameter("password", room.Password);
                dbClient.AddParameter("state", AccessStr);
                dbClient.AddParameter("tags", formattedTags.ToString());
                dbClient.RunQuery();
            }

            room.GetGameMap().GenerateMaps();

            if (session.GetHabbo().CurrentRoom == null)
            {
                session.SendPacket(new RoomSettingsSavedComposer(room.RoomId));
                session.SendPacket(new RoomInfoUpdatedComposer(room.RoomId));
                session.SendPacket(new RoomVisualizationSettingsComposer(room.WallThickness, room.FloorThickness, PlusEnvironment.EnumToBool(room.Hidewall.ToString())));
            }
            else
            {
                room.SendPacket(new RoomSettingsSavedComposer(room.RoomId));
                room.SendPacket(new RoomInfoUpdatedComposer(room.RoomId));
                room.SendPacket(new RoomVisualizationSettingsComposer(room.WallThickness, room.FloorThickness, PlusEnvironment.EnumToBool(room.Hidewall.ToString())));
            }
            
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModDoorModeSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModWalkthroughSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModChatScrollSpeedSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModChatFloodFilterSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModChatHearRangeSeen", 1);
        }
    }
}
