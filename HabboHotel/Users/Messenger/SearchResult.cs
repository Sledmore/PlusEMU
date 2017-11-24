using Plus.Communication.Packets.Incoming;

namespace Plus.HabboHotel.Users.Messenger
{
    public struct SearchResult
    {
        public int UserId;
        public string Username;
        public string Motto;
        public string Figure;
        public string LastOnline;

        public SearchResult(int UserId, string Username, string Motto, string Figure, string LastOnline)
        {
            this.UserId = UserId;
            this.Username = Username;
            this.Motto = Motto;
            this.Figure = Figure;
            this.LastOnline = LastOnline;
        }
    }
}