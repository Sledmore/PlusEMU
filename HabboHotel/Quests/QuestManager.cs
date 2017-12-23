using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Incoming;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Quests;

using Plus.Database.Interfaces;
using log4net;

namespace Plus.HabboHotel.Quests
{
    public class QuestManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Quests.QuestManager");

        private Dictionary<int, Quest> _quests;
        private Dictionary<string, int> _questCount;

        public QuestManager()
        {
            _quests = new Dictionary<int, Quest>();
            _questCount = new Dictionary<string, int>();
        }

        public void Init()
        {
            if (_quests.Count > 0)
            _quests.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`type`,`level_num`,`goal_type`,`goal_data`,`action`,`pixel_reward`,`data_bit`,`reward_type`,`timestamp_unlock`,`timestamp_lock` FROM `quests`");
                DataTable dTable = dbClient.GetTable();

                if (dTable != null)
                {
                    foreach (DataRow dRow in dTable.Rows)
                    {
                        int id = Convert.ToInt32(dRow["id"]);
                        string category = Convert.ToString(dRow["type"]);
                        int num = Convert.ToInt32(dRow["level_num"]);
                        int type = Convert.ToInt32(dRow["goal_type"]);
                        int goalData = Convert.ToInt32(dRow["goal_data"]);
                        string name = Convert.ToString(dRow["action"]);
                        int reward = Convert.ToInt32(dRow["pixel_reward"]);
                        string dataBit = Convert.ToString(dRow["data_bit"]);
                        int rewardtype = Convert.ToInt32(dRow["reward_type"].ToString());
                        int time = Convert.ToInt32(dRow["timestamp_unlock"]);
                        int locked = Convert.ToInt32(dRow["timestamp_lock"]);

                        _quests.Add(id, new Quest(id, category, num, (QuestType)type, goalData, name, reward, dataBit, rewardtype, time, locked));
                        AddToCounter(category);
                    }
                }
            }

            log.Info("Quest Manager -> LOADED");
        }

        private void AddToCounter(string category)
        {
            int count = 0;
            if (_questCount.TryGetValue(category, out count))
            {
                _questCount[category] = count + 1;
            }
            else
            {
                _questCount.Add(category, 1);
            }
        }

        public Quest GetQuest(int id)
        {
            _quests.TryGetValue(id, out Quest quest);
            return quest;
        }

        public int GetAmountOfQuestsInCategory(string category)
        {
            _questCount.TryGetValue(category, out int count);
            return count;
        }

        public void ProgressUserQuest(GameClient session, QuestType type, int data = 0)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetStats().QuestId <= 0)
            {
                return;
            }

            Quest quest = GetQuest(session.GetHabbo().GetStats().QuestId);

            if (quest == null || quest.GoalType != type)
            {
                return;
            }

            int currentProgress = session.GetHabbo().GetQuestProgress(quest.Id);
            int totalProgress = currentProgress;
            bool completeQuest = false;

            switch (type)
            {
                default:

                    totalProgress++;

                    if (totalProgress >= quest.GoalData)
                    {
                        completeQuest = true;
                    }

                    break;

                case QuestType.ExploreFindItem:

                    if (data != quest.GoalData)
                        return;

                    totalProgress = Convert.ToInt32(quest.GoalData);
                    completeQuest = true;
                    break;

                case QuestType.StandOn:

                    if (data != quest.GoalData)
                        return;

                    totalProgress = Convert.ToInt32(quest.GoalData);
                    completeQuest = true;
                    break;

                case QuestType.XmasParty:
                    totalProgress++;
                    if (totalProgress == quest.GoalData)
                        completeQuest = true;
                    break;

                case QuestType.GiveItem:

                    if (data != quest.GoalData)
                        return;

                    totalProgress = Convert.ToInt32(quest.GoalData);
                    completeQuest = true;
                    break;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_quests` SET `progress` = '" + totalProgress + "' WHERE `user_id` = '" + session.GetHabbo().Id + "' AND `quest_id` = '" + quest.Id + "' LIMIT 1");

                if (completeQuest)
                    dbClient.RunQuery("UPDATE `user_stats` SET `quest_id` = '0' WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
            }

            session.GetHabbo().quests[session.GetHabbo().GetStats().QuestId] = totalProgress;
            session.SendPacket(new QuestStartedComposer(session, quest));

            if (completeQuest)
            {
                session.GetHabbo().GetMessenger().BroadcastAchievement(session.GetHabbo().Id, Users.Messenger.MessengerEventTypes.QuestCompleted, quest.Category + "." + quest.Name);

                session.GetHabbo().GetStats().QuestId = 0;
                session.GetHabbo().QuestLastCompleted = quest.Id;
                session.SendPacket(new QuestCompletedComposer(session, quest));
                session.GetHabbo().Duckets += quest.Reward;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, quest.Reward));
                GetList(session, null);
            }
        }

        public Quest GetNextQuestInSeries(string category, int number)
        {
            foreach (Quest quest in _quests.Values)
            {
                if (quest.Category == category && quest.Number == number)
                {
                    return quest;
                }
            }

            return null;
        }

        public void GetList(GameClient session, ClientPacket message)
        {
            Dictionary<string, int> UserQuestGoals = new Dictionary<string, int>();
            Dictionary<string, Quest> UserQuests = new Dictionary<string, Quest>();

            foreach (Quest quest in _quests.Values.ToList())
            {
                if (quest.Category.Contains("xmas2012"))
                    continue;

                if (!UserQuestGoals.ContainsKey(quest.Category))
                {
                    UserQuestGoals.Add(quest.Category, 1);
                    UserQuests.Add(quest.Category, null);
                }

                if (quest.Number >= UserQuestGoals[quest.Category])
                {
                    int UserProgress = session.GetHabbo().GetQuestProgress(quest.Id);

                    if (session.GetHabbo().GetStats().QuestId != quest.Id && UserProgress >= quest.GoalData)
                    {
                        UserQuestGoals[quest.Category] = quest.Number + 1;
                    }
                }
            }

            foreach (Quest quest in _quests.Values.ToList())
            {
                foreach (var Goal in UserQuestGoals)
                {
                    if (quest.Category.Contains("xmas2012"))
                        continue;

                    if (quest.Category == Goal.Key && quest.Number == Goal.Value)
                    {
                        UserQuests[Goal.Key] = quest;
                        break;
                    }
                }
            }

            session.SendPacket(new QuestListComposer(session, (message != null), UserQuests));
        }

        public void QuestReminder(GameClient session, int questId)
        {
            Quest Quest = GetQuest(questId);
            if (Quest == null)
                return;

            session.SendPacket(new QuestStartedComposer(session, Quest));
        }
    }
}