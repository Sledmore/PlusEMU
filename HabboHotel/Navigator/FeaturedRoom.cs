using System;

namespace Plus.HabboHotel.Navigator
{
    public class FeaturedRoom
    {
        public int RoomId { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public FeaturedRoom(int RoomId, string Caption, string Description, string Image)
        {
            this.RoomId = RoomId;
            this.Caption = Caption;
            this.Description = Description;
            this.Image = Image;
        }
    }
}