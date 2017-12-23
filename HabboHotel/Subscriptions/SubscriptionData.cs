namespace Plus.HabboHotel.Subscriptions
{
    public class SubscriptionData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Badge { get; private set; }
        public int Credits { get; private set; }
        public int Duckets { get; private set; }
        public int Respects { get; private set; }

        public SubscriptionData(int id, string name, string badge, int credits, int duckets, int respects)
        {
            Id = id;
            Name = name;
            Badge = badge;
            Credits = credits;
            Duckets = duckets;
            Respects = respects;
        }
    }
}
