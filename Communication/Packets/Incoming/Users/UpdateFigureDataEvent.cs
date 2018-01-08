using System;
using System.Linq;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Users
{
    class UpdateFigureDataEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            string gender = packet.PopString().ToUpper();
            string look = PlusEnvironment.GetFigureManager().ProcessFigure(packet.PopString(), gender, session.GetHabbo().GetClothing().GetClothingParts, true);

            if (look == session.GetHabbo().Look)
                return;

            if ((DateTime.Now - session.GetHabbo().LastClothingUpdateTime).TotalSeconds <= 2.0)
            {
                session.GetHabbo().ClothingUpdateWarnings += 1;
                if (session.GetHabbo().ClothingUpdateWarnings >= 25)
                    session.GetHabbo().SessionClothingBlocked = true;
                return;
            }

            if (session.GetHabbo().SessionClothingBlocked)
                return;

            session.GetHabbo().LastClothingUpdateTime = DateTime.Now;

            string[] allowedGenders = { "M", "F" };
            if (!allowedGenders.Contains(gender))
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Sorry, you chose an invalid gender."));
                return;
            }

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ProfileChangeLook);

            session.GetHabbo().Look = PlusEnvironment.FilterFigure(look);
            session.GetHabbo().Gender = gender.ToLower();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `look` = @look, `gender` = @gender WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("look", look);
                dbClient.AddParameter("gender", gender);
                dbClient.RunQuery();
            }

            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_AvatarLooks", 1);
            session.SendPacket(new AvatarAspectUpdateComposer(look, gender));
            if (session.GetHabbo().Look.Contains("ha-1006"))
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.WearHat);

            if (session.GetHabbo().InRoom)
            {
                RoomUser roomUser = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
                if (roomUser != null)
                {
                    session.SendPacket(new UserChangeComposer(roomUser, true));
                    session.GetHabbo().CurrentRoom.SendPacket(new UserChangeComposer(roomUser, false));
                }
            }
        }
    }
}
