namespace Plus.HabboHotel.Groups
{
    public class GroupColours
    {
        public int Id { get; }
        public string Colour { get; }

        public GroupColours(int id, string colour)
        {
            Id = id;
            Colour = colour;
        }
    }
}
