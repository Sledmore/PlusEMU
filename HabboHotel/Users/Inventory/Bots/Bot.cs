namespace Plus.HabboHotel.Users.Inventory.Bots
{
    public class Bot
    {
        private int _id;
        private int _ownerId;
        private string _name;
        private string _motto;
        private string _figure;
        private string _gender;

        public Bot(int Id, int OwnerId, string Name, string Motto, string Figure, string Gender)
        {
            this.Id = Id;
            this.OwnerId = OwnerId;
            this.Name = Name;
            this.Motto = Motto;
            this.Figure = Figure;
            this.Gender = Gender;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public int OwnerId
        {
            get { return this._ownerId; }
            set { this._ownerId = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public string Motto
        {
            get { return this._motto; }
            set { this._motto = value; }
        }

        public string Figure
        {
            get { return this._figure; }
            set { this._figure = value; }
        }

        public string Gender
        {
            get { return this._gender; }
            set { this._gender = value; }
        }
    }
}
