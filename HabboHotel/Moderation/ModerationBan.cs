using System;

namespace Plus.HabboHotel.Moderation
{ 
    public class ModerationBan
    {
        public string Value { get; set; }
        public double Expire { get; set; }
        public string Reason { get; set; }
        public ModerationBanType Type { get; set; }

        public ModerationBan(ModerationBanType type, string value, string reason, Double expire)
        {
            this.Type = type;
            this.Value = value;
            this.Reason = reason;
            this.Expire = expire;
        }

        public bool Expired
        {
            get
            {
                if (PlusEnvironment.GetUnixTimestamp() >= Expire)
                    return true;
                return false;
            }
        }
    }
}