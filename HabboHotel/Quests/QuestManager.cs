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

            this.Init();
        }

        public void Init()
        {
            if (this._quests.Count > 0)
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

        public Quest GetQuest(int Id)
        {
            Quest quest = null;
            _quests.TryGetValue(Id, out quest);
            return quest;
        }

        public int GetAmountOfQuestsInCategory(string Category)
        {
            int count = 0;
            _questCount.TryGetValue(Category, out count);
            return count;
        }

        public void ProgressUserQuest(GameClient Session, QuestType QuestType, int EventData = 0)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetStats().QuestID <= 0)
            {
                return;
            }

            Quest UserQuest = GetQuest(Session.GetHabbo().GetStats().QuestID);

            if (UserQuest == null || UserQuest.GoalType != QuestType)
            {
                return;
            }

            int CurrentProgress = Session.GetHabbo().GetQuestProgress(UserQuest.Id);
            int NewProgress = CurrentProgress;
            bool PassQuest = false;

            switch (QuestType)
            {
                default:

                    NewProgress++;

                    if (NewProgress >= UserQuest.GoalData)
                    {
                        PassQuest = true;
                    }

                    break;

                case QuestType.EXPLORE_FIND_ITEM:

                    if (EventData != UserQuest.GoalData)
                        return;

                    NewProgress = Convert.ToInt32(UserQuest.GoalData);
                    PassQuest = true;
                    break;

                case QuestType.STAND_ON:

                    if (EventData != UserQuest.GoalData)
                        return;

                    NewProgress = Convert.ToInt32(UserQuest.GoalData);
                    PassQuest = true;
                    break;

                case QuestType.XMAS_PARTY:
                    NewProgress++;
                    if (NewProgress == UserQuest.GoalData)
                        PassQuest = true;
                    break;

                case QuestType.GIVE_ITEM:

                    if (EventData != UserQuest.GoalData)
                        return;

                    NewProgress = Convert.ToInt32(UserQuest.GoalData);
                    PassQuest = true;
                    break;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_quests` SET `progress` = '" + NewProgress + "' WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `quest_id` = '" + UserQuest.Id + "' LIMIT 1");

                if (PassQuest)
                    dbClient.RunQuery("UPDATE `user_stats` SET `quest_id` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            Session.GetHabbo().quests[Session.GetHabbo().GetStats().QuestID] = NewProgress;
            Session.SendPacket(new QuestStartedComposer(Session, UserQuest));

            if (PassQuest)
            {
                Session.GetHabbo().GetMessenger().BroadcastAchievement(Session.GetHabbo().Id, Users.Messenger.MessengerEventTypes.QUEST_COMPLETED, UserQuest.Category + "." + UserQuest.Name);

                Session.GetHabbo().GetStats().QuestID = 0;
                Session.GetHabbo().QuestLastCompleted = UserQuest.Id;
                Session.SendPacket(new QuestCompletedComposer(Session, UserQuest));
                Session.GetHabbo().Duckets += UserQuest.Reward;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, UserQuest.Reward));
                GetList(Session, null);
            }
        }

        public Quest GetNextQuestInSeries(string Category, int Number)
        {
            foreach (Quest Quest in _quests.Values)
            {
                if (Quest.Category == Category && Quest.Number == Number)
                {
                    return Quest;
                }
            }

            return null;
        }

        public void GetList(GameClient Session, ClientPacket Message)
        {
            Dictionary<string, int> UserQuestGoals = new Dictionary<string, int>();
            Dictionary<string, Quest> UserQuests = new Dictionary<string, Quest>();

            foreach (Quest Quest in _quests.Values.ToList())
            {
                if (Quest.Category.Contains("xmas2012"))
                    continue;

                if (!UserQuestGoals.ContainsKey(Quest.Category))
                {
                    UserQuestGoals.Add(Quest.Category, 1);
                    UserQuests.Add(Quest.Category, null);
                }

                if (Quest.Number >= UserQuestGoals[Quest.Category])
                {
                    int UserProgress = Session.GetHabbo().GetQuestProgress(Quest.Id);

                    if (Session.GetHabbo().GetStats().QuestID != Quest.Id && UserProgress >= Quest.GoalData)
                    {
                        UserQuestGoals[Quest.Category] = Quest.Number + 1;
                    }
                }
            }

            foreach (Quest Quest in _quests.Values.ToList())
            {
                foreach (var Goal in UserQuestGoals)
                {
                    if (Quest.Category.Contains("xmas2012"))
                        continue;

                    if (Quest.Category == Goal.Key && Quest.Number == Goal.Value)
                    {
                        UserQuests[Goal.Key] = Quest;
                        break;
                    }
                }
            }

            Session.SendPacket(new QuestListComposer(Session, (Message != null), UserQuests));
        }

        public void QuestReminder(GameClient Session, int QuestId)
        {
            Quest Quest = GetQuest(QuestId);
            if (Quest == null)
                return;

            Session.SendPacket(new QuestStartedComposer(Session, Quest));
        }
    }
}