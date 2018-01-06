namespace Plus.HabboHotel.Badges
{
    public class BadgeDefinition
    {
        public string Code { get; }
        public string RequiredRight { get; }

        public BadgeDefinition(string code, string requiredRight)
        {
            Code = code;
            RequiredRight = requiredRight;
        }
    }
}