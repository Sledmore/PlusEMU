using System;

namespace Plus.HabboHotel.Rooms
{
    public class RoomPromotion
    {
        private string _name;
        private string _description;
        private double _timestampExpires;
        private double _timestampStarted;
        private int _categoryId;

        public RoomPromotion(string name, string description, int categoryId)
        {
            _name = name;
            _description = description;
            _timestampStarted = PlusEnvironment.GetUnixTimestamp();
            _timestampExpires = (PlusEnvironment.GetUnixTimestamp()) + (Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("room.promotion.lifespan")) * 60);
            _categoryId = categoryId;
        }

        public RoomPromotion(string name, string description, double started, double expires, int categoryId)
        {
            _name = name;
            _description = description;
            _timestampStarted = started;
            _timestampExpires = expires;
            _categoryId = categoryId;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public double TimestampStarted
        {
            get { return _timestampStarted; }
        }

        public double TimestampExpires
        {
            get { return _timestampExpires; }
            set { _timestampExpires = value; }
        }

        public bool HasExpired
        {
            get { return (TimestampExpires - PlusEnvironment.GetUnixTimestamp()) < 0; }
        }

        public int MinutesLeft
        {
            get { return Convert.ToInt32(Math.Ceiling((TimestampExpires - PlusEnvironment.GetUnixTimestamp()) / 60)); }
        }

        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }
    }
}