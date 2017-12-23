namespace Plus.HabboHotel.Navigator
{
    public class FeaturedRoom
    {
        public int RoomId { get; private set; }
        public string Caption { get; private set; }
        public string Description { get; private set; }
        public string Image { get; private set; }

        public FeaturedRoom(int roomId, string caption, string description, string images)
        {
            RoomId = roomId;
            Caption = caption;
            Description = description;
            Image = images;
        }
    }
}