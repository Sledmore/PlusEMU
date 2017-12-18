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

        public bool ProgressAchievement(GameClient session, string group, int progress, bool fromBeginning = false)
        {
            if (!_achievements.ContainsKey(group) || session == null)
                return false;

            Achievement data = null;
            data = _achievements[group];

            UserAchievement userData = session.GetHabbo().GetAchievementData(group);
            if (userData == null)
            {
                userData = new UserAchievement(group, 0, 0);
                session.GetHabbo().Achievements.TryAdd(group, userData);
            }

            int TotalLevels = data.Levels.Count;

            if (userData != null && userData.Level == TotalLevels)
                return false; // done, no more.

            int TargetLevel = (userData != null ? userData.Level + 1 : 1);

            if (TargetLevel > TotalLevels)
                TargetLevel = TotalLevels;

            AchievementLevel TargetLevelData = data.Levels[TargetLevel];
            int NewProgress = 0;
            if (fromBeginning)
                NewProgress = progress;
            else
                NewProgress = (userData != null ? userData.Progress + progress : progress);

            int NewLevel = (userData != null ? userData.Level : 0);
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
                    session.GetHabbo().GetBadgeComponent().GiveBadge(group + TargetLevel, true, session);
                else
                {
                    session.GetHabbo().GetBadgeComponent().RemoveBadge(Convert.ToString(group + (TargetLevel - 1)));
                    session.GetHabbo().GetBadgeComponent().GiveBadge(group + TargetLevel, true, session);
                }

                if (NewTarget > TotalLevels)
                {
                    NewTarget = TotalLevels;
                }


                session.SendPacket(new AchievementUnlockedComposer(data, TargetLevel, TargetLevelData.RewardPoints, TargetLevelData.RewardPixels));
                session.GetHabbo().GetMessenger().BroadcastAchievement(session.GetHabbo().Id, Users.Messenger.MessengerEventTypes.AchievementUnlocked, group + TargetLevel);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES ('" + session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", group);
                    dbClient.RunQuery();
                }

                userData.Level = NewLevel;
                userData.Progress = NewProgress;

                session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                session.GetHabbo().GetStats().AchievementPoints += TargetLevelData.RewardPoints;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, TargetLevelData.RewardPixels));
                session.SendPacket(new AchievementScoreComposer(session.GetHabbo().GetStats().AchievementPoints));

                AchievementLevel NewLevelData = data.Levels[NewTarget];
                session.SendPacket(new AchievementProgressedComposer(data, NewTarget, NewLevelData, TotalLevels, session.GetHabbo().GetAchievementData(group)));

                return true;
            }
            else
            {
                userData.Level = NewLevel;
                userData.Progress = NewProgress;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES ('" + session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", group);
                    dbClient.RunQuery();
                }

                session.SendPacket(new AchievementProgressedComposer(data, TargetLevel, TargetLevelData, TotalLevels, session.GetHabbo().GetAchievementData(group)));
            }
            return false;
        }

        public ICollection<Achievement> GetGameAchievements(int gameId)
        {
            List<Achievement> achievements = new List<Achievement>();

            foreach (Achievement achievement in _achievements.Values.ToList())
            {
                if (achievement.Category == "games" && achievement.GameId == gameId)
                    achievements.Add(achievement);
            }

            return achievements;
        }
    }
}