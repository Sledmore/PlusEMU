namespace Plus.HabboHotel.Rooms.Chat.Styles
{
    public sealed class ChatStyle
    {
        private int _id;
        private string _name;
        private string _requiredRight;

        public ChatStyle(int Id, string Name, string RequiredRight)
        {
            this._id = Id;
            this._name = Name;
            this._requiredRight = RequiredRight;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public string RequiredRight
        {
            get { return this._requiredRight; }
            set { this._requiredRight = value; }
        }
    }
}