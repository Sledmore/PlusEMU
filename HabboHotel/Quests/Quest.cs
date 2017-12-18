namespace Plus.HabboHotel.Quests
{
    public class Quest
    {
        public int Id { get; private set; }
        public string Category { get; private set; }
        public string DataBit { get; private set; }
        public int GoalData { get; private set; }
        public QuestType GoalType { get; private set; }
        public bool HasEnded { get; private set; }
        public string Name { get; private set; }
        public int Number { get; private set; }
        public int Reward { get; private set; }
        public int RewardType { get; private set; }
        public int TimeUnlock { get; private set; }

        public Quest(int id, string category, int number, QuestType goalType, int goalData, string name, int reward, string dataBit, int rewardType, int timeUnlock, int timeLock)
        {
            Id = id;
            Category = category;
            Number = number;
            GoalType = goalType;
            GoalData = goalData;
            Name = name;
            Reward = reward;
            DataBit = dataBit;
            RewardType = rewardType;
            TimeUnlock = timeUnlock;
            HasEnded = (timeLock >= Utilities.UnixTimestamp.GetNow() && timeLock > 0) ? true : false;
        }

        public string ActionName
        {
            get { return QuestTypeUtillity.GetString(GoalType); }
        }

        public bool IsCompleted(int progress)
        {
            switch (GoalType)
            {
                default:
                    return (progress >= GoalData);
                case QuestType.ExploreFindItem:
                    return (progress >= 1);
            }
        }
    }
}