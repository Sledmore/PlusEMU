using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Moderation
{
    public class ModerationTicket
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Category { get; set; }
        public double Timestamp { get; set; }
        public int Priority { get; set; }
        public bool Answered { get; set; }
        public Habbo Sender { get; set; }
        public Habbo Reported { get; set; }
        public Habbo Moderator { get; set; }
        public string Issue { get; set; }
        public RoomData Room { get; set; }

        public List<string> ReportedChats;

        public ModerationTicket(int id, int type, int category, double timestamp, int priority, Habbo sender, Habbo reported, string issue, RoomData room, List<string> reportedChats)
        {
            this.Id = id;
            this.Type = type;
            this.Category = category;
            this.Timestamp = timestamp;
            this.Priority = priority;
            this.Sender = sender;
            this.Reported = reported;
            this.Moderator = null;
            this.Issue = issue;
            this.Room = room;
            this.Answered = false;
            this.ReportedChats = reportedChats;
        }

        public int GetStatus(int Id)
        {
            if (Moderator == null)
                return 1;         
            else if (Moderator.Id == Id && !Answered)
                return 2;
            else if (Answered)
                return 3;
            else
                return 3;           
        }
    }
}
