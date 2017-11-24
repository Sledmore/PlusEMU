using System;
using System.Data;

namespace Plus.HabboHotel.Users.Authenticator
{
    public static class HabboFactory
    {
        public static Habbo GenerateHabbo(DataRow Row, DataRow UserInfo)
        {
            return new Habbo(Convert.ToInt32(Row["id"]), Convert.ToString(Row["username"]), Convert.ToInt32(Row["rank"]), Convert.ToString(Row["motto"]), Convert.ToString(Row["look"]),
                Convert.ToString(Row["gender"]), Convert.ToInt32(Row["credits"]), Convert.ToInt32(Row["activity_points"]), 
                Convert.ToInt32(Row["home_room"]), PlusEnvironment.EnumToBool(Row["block_newfriends"].ToString()), Convert.ToInt32(Row["last_online"]),
                PlusEnvironment.EnumToBool(Row["hide_online"].ToString()), PlusEnvironment.EnumToBool(Row["hide_inroom"].ToString()),
                Convert.ToDouble(Row["account_created"]), Convert.ToInt32(Row["vip_points"]),Convert.ToString(Row["machine_id"]), Convert.ToString(Row["volume"]),
                PlusEnvironment.EnumToBool(Row["chat_preference"].ToString()), PlusEnvironment.EnumToBool(Row["focus_preference"].ToString()), PlusEnvironment.EnumToBool(Row["pets_muted"].ToString()), PlusEnvironment.EnumToBool(Row["bots_muted"].ToString()),
                PlusEnvironment.EnumToBool(Row["advertising_report_blocked"].ToString()), Convert.ToDouble(Row["last_change"].ToString()), Convert.ToInt32(Row["gotw_points"]),
                PlusEnvironment.EnumToBool(Convert.ToString(Row["ignore_invites"])), Convert.ToDouble(Row["time_muted"]), Convert.ToDouble(UserInfo["trading_locked"]),
                PlusEnvironment.EnumToBool(Row["allow_gifts"].ToString()), Convert.ToInt32(Row["friend_bar_state"]),  PlusEnvironment.EnumToBool(Row["disable_forced_effects"].ToString()),
                PlusEnvironment.EnumToBool(Row["allow_mimic"].ToString()), Convert.ToInt32(Row["rank_vip"]));
        }
    }
}