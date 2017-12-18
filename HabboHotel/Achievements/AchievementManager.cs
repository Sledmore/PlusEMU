using System;
using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;

using Plus.Database.Interfaces;
using log4net;

namespace Plus.HabboHotel.Achievements
{
    public class AchievementManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Achievements.AchievementManager");

        public Dictionary<string, Achievement> _achievements;

        public AchievementManager()
        {
            this._achievements = new Dictionary<string, Achievement>();
            this.LoadAchievements();

            log.Info("Achievement Manager -> LOADED");
        }

        public void LoadAchievements()
        {
            AchievementLevelFactory.GetAchievementLevels(out _achievements);
        }

        public bool ProgressAchievement(GameClient Session, string AchievementGroup, int ProgressAmount, bool FromZero = false)
        {
            if (!_achievements.ContainsKey(AchievementGroup) || Session == null)
                return false;

            Achievement AchievementData = null;
            AchievementData = _achievements[AchievementGroup];

            UserAchievement UserData = Session.GetHabbo().GetAchievementData(AchievementGroup);
            if (UserData == null)
            {
                UserData = new UserAchievement(AchievementGroup, 0, 0);
                Session.GetHabbo().Achievements.TryAdd(AchievementGroup, UserData);
            }

            int TotalLevels = AchievementData.Levels.Count;

            if (UserData != null && UserData.Level == TotalLevels)
                return false; // done, no more.

            int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

            if (TargetLevel > TotalLevels)
                TargetLevel = TotalLevels;

            AchievementLevel TargetLevelData = AchievementData.Levels[TargetLevel];
            int NewProgress = 0;
            if (FromZero)
                NewProgress = ProgressAmount;
            else
                NewProgress = (UserData != null ? UserData.Progress + ProgressAmount : ProgressAmount);

            int NewLevel = (UserData != null ? UserData.Level : 0);
            int NewTarget = NewLevel + 1;

            if (NewTarget > TotalLevels)
                NewTarget = TotalLevels;

            if (NewProgress >= TargetLevelData.Requirement)
            {
                NewLevel++;
                NewTarget++;

                int ProgressRemainder = NewProgress - TargetLevelData.Requirement;

                NewProgress = 0;

                if (TargetLevel == 1)
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(AchievementGroup + TargetLevel, true, Session);
                else
                {
                    Session.GetHabbo().GetBadgeComponent().RemoveBadge(Convert.ToString(AchievementGroup + (TargetLevel - 1)));
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(AchievementGroup + TargetLevel, true, Session);
                }

                if (NewTarget > TotalLevels)
                {
                    NewTarget = TotalLevels;
                }


                Session.SendPacket(new AchievementUnlockedComposer(AchievementData, TargetLevel, TargetLevelData.RewardPoints, TargetLevelData.RewardPixels));
                Session.GetHabbo().GetMessenger().BroadcastAchievement(Session.GetHabbo().Id, Users.Messenger.MessengerEventTypes.AchievementUnlocked, AchievementGroup + TargetLevel);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                }

                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;

                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.GetHabbo().GetStats().AchievementPoints += TargetLevelData.RewardPoints;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, TargetLevelData.RewardPixels));
                Session.SendPacket(new AchievementScoreComposer(Session.GetHabbo().GetStats().AchievementPoints));

                AchievementLevel NewLevelData = AchievementData.Levels[NewTarget];
                Session.SendPacket(new AchievementProgressedComposer(AchievementData, NewTarget, NewLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));

                return true;
            }
            else
            {
                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                }

                Session.SendPacket(new AchievementProgressedComposer(AchievementData, TargetLevel, TargetLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));
            }
            return false;
        }

        public ICollection<Achievement> GetGameAchievements(int GameId)
        {
            List<Achievement> GameAchievements = new List<Achievement>();
            foreach (Achievement Achievement in _achievements.Values.ToList())
            {
                if (Achievement.Category == "games" && Achievement.GameId == GameId)
                    GameAchievements.Add(Achievement);
            }
            return GameAchievements;
        }
    }
}