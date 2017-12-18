using Plus.Utilities;

namespace Plus.HabboHotel.Rewards
{
    public class Reward
    {
        public double RewardStart { get; set; }
        public double RewardEnd { get; set; }
        public RewardType Type { get; set; }
        public string RewardData { get; set; }
        public string Message { get; set; }

        public Reward(double start, double end, string type, string rewardData, string message)
        {
            RewardStart = start;
            RewardEnd = end;
            Type = RewardTypeUtility.GetType(type);
            RewardData = rewardData;
            Message = message;
        }

        public bool Active
        {
            get
            {
                double Now = UnixTimestamp.GetNow();
                return (Now >= RewardStart && Now <= RewardEnd);
            }
        }
    }
}
