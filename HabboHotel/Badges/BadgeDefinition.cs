namespace Plus.HabboHotel.Badges
{
    public class BadgeDefinition
    {
        public string Code { get; private set; }
        public string RequiredRight { get; private set; }

        public BadgeDefinition(string code, string requiredRight)
        {
            Code = code;
            RequiredRight = requiredRight;
        }
    }
}