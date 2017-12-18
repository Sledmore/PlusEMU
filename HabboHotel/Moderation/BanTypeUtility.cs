using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.HabboHotel.Moderation
{
    public static class BanTypeUtility
    {
        public static ModerationBanType GetModerationBanType(string type)
        {
            switch (type)
            {
                default:
                case "user":
                    return ModerationBanType.Username;
                case "ip":
                    return ModerationBanType.IP;
                case "machine":
                    return ModerationBanType.Machine;
            }
        }

        public static string FromModerationBanType(ModerationBanType type)
        {
            switch (type)
            {
                default:
                case ModerationBanType.Username:
                    return "user";
                case ModerationBanType.IP:
                    return "ip";
                case ModerationBanType.Machine:
                    return "machine";
            }
        }
    }
}
