namespace Plus.HabboHotel.Rewards
{
    public class RewardTypeUtility
    {
        public static RewardType GetType(string type)
        {
            switch (type.ToLower())
            {
                case "badge":
                    return RewardType.Badge;

                case "credits":
                    return RewardType.Credits;

                case "duckets":
                    return RewardType.Duckets;

                case "diamonds":
                    return RewardType.Diamonds;

                default:
                case "none":
                    return RewardType.None;
            }
        }
    }
}
