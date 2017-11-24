namespace Plus.HabboHotel.Quests
{
    public class Quest
    {
        public readonly string Category;
        public readonly string DataBit;
        public readonly int GoalData;
        public readonly QuestType GoalType;
        public readonly bool HasEnded;
        public readonly int Id;
        public readonly string Name;
        public readonly int Number;
        public readonly int Reward;
        public readonly int RewardType;
        public readonly int TimeUnlock;

        public Quest(int Id, string Category, int Number, QuestType GoalType, int GoalData, string Name, int Reward, string DataBit, int RewardType, int TimeUnlock, int TimeLock)
        {
            this.Id = Id;
            this.Category = Category;
            this.Number = Number;
            this.GoalType = GoalType;
            this.GoalData = GoalData;
            this.Name = Name;
            this.Reward = Reward;
            this.DataBit = DataBit;
            this.RewardType = RewardType;
            this.TimeUnlock = TimeUnlock;
            this.HasEnded = (TimeLock >= PlusEnvironment.GetUnixTimestamp() && TimeLock > 0) ? true : false;
        }

        public string ActionName
        {
            get { return QuestTypeUtillity.GetString(GoalType); }
        }

        public bool IsCompleted(int UserProgress)
        {
            switch (GoalType)
            {
                default:
                    return (UserProgress >= GoalData);
                case QuestType.EXPLORE_FIND_ITEM:
                    return (UserProgress >= 1);
            }
        }
    }
}