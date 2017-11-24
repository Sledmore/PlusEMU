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
        public Reward(double Start, double End, string Type, string RewardData, string Message)
        {
            this.RewardStart = Start;
            this.RewardEnd = End;
            this.Type = RewardTypeUtility.GetType(Type);
            this.RewardData = RewardData;
            this.Message = Message;
        }

        public bool isActive()
        {
            double Now = UnixTimestamp.GetNow();
            return (Now >= RewardStart && Now <= RewardEnd);
        }
    }
}
