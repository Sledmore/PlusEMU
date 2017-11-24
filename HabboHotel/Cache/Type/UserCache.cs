using System;

namespace Plus.HabboHotel.Cache.Type
{
    public class UserCache
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Motto { get; set; }
        public string Look { get; set; }
        public DateTime AddedTime { get; set; }
        public UserCache(int Id, string Username, string Motto, string Look)
        {
            this.Id = Id;
            this.Username = Username;
            this.Motto = Motto;
            this.Look = Look;
            this.AddedTime = DateTime.Now;
        }
        public bool isExpired()
        {
            TimeSpan CacheTime = DateTime.Now - this.AddedTime;
            return CacheTime.TotalMinutes >= 30;
        }
    }
}
