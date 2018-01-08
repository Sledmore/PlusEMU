namespace Plus.HabboHotel.Users.Messenger
{
    public class MessengerRequest
    {
        private int _toUser;
        private int _fromUser;
        private string _username;

        public MessengerRequest(int ToUser, int FromUser, string Username)
        {
            _toUser = ToUser;
            _fromUser = FromUser;
            _username = Username;
        }

        public string Username
        {
            get { return _username; }
        }

        public int To
        {
            get { return _toUser; }
        }

        public int From
        {
            get { return _fromUser; }
        }
    }
}