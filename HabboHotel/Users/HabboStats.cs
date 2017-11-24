namespace Plus.HabboHotel.Users
{
    public class HabboStats
    {
        public int RoomVisits { get; set; }
        public double OnlineTime { get; set; }
        public int Respect { get; set; }
        public int RespectGiven { get; set; }
        public int GiftsGiven { get; set; }
        public int GiftsReceived { get; set; }
        public int DailyRespectPoints { get; set; }
        public int DailyPetRespectPoints { get; set; }
        public int AchievementPoints { get; set; }
        public int QuestID { get; set; }
        public int QuestProgress { get; set; }
        public int FavouriteGroupId { get; set; }
        public string RespectsTimestamp { get; set; }
        public int ForumPosts { get; set; }

        public HabboStats(int roomVisits, double onlineTime, int Respect, int respectGiven, int giftsGiven, int giftsReceived, int dailyRespectPoints, int dailyPetRespectPoints, int achievementPoints, int questID, int questProgress, int groupID, string RespectsTimestamp, int ForumPosts)
        {
            this.RoomVisits = roomVisits;
            this.OnlineTime = onlineTime;
            this.Respect = Respect;
            this.RespectGiven = respectGiven;
            this.GiftsGiven = giftsGiven;
            this.GiftsReceived = giftsReceived;
            this.DailyRespectPoints = dailyRespectPoints;
            this.DailyPetRespectPoints = dailyPetRespectPoints;
            this.AchievementPoints = achievementPoints;
            this.QuestID = questID;
            this.QuestProgress = questProgress;
            this.FavouriteGroupId = groupID;
            this.RespectsTimestamp = RespectsTimestamp;
            this.ForumPosts = ForumPosts;
        }
    }
}
