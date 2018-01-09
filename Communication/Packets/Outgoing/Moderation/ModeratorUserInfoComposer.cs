using System;
using System.Data;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserInfoComposer : ServerPacket
    {
        public ModeratorUserInfoComposer(DataRow User, DataRow Info)
            : base(ServerPacketHeader.ModeratorUserInfoMessageComposer)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(Info["trading_locked"]));


            WriteInteger(User != null ? Convert.ToInt32(User["id"]) : 0);
           WriteString(User != null ? Convert.ToString(User["username"]) : "Unknown");
           WriteString(User != null ? Convert.ToString(User["look"]) : "Unknown");
            WriteInteger(User != null ? Convert.ToInt32(Math.Ceiling((PlusEnvironment.GetUnixTimestamp() - Convert.ToDouble(User["account_created"])) / 60)) : 0);
            WriteInteger(User != null ? Convert.ToInt32(Math.Ceiling((PlusEnvironment.GetUnixTimestamp() - Convert.ToDouble(User["last_online"])) / 60)) : 0);
            WriteBoolean(User != null ? PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Convert.ToInt32(User["id"])) != null : false);
            WriteInteger(Info != null ? Convert.ToInt32(Info["cfhs"]) : 0);
            WriteInteger(Info != null ? Convert.ToInt32(Info["cfhs_abusive"]) : 0);
            WriteInteger(Info != null ? Convert.ToInt32(Info["cautions"]) : 0);
            WriteInteger(Info != null ? Convert.ToInt32(Info["bans"]) : 0);
            WriteInteger(Info != null ? Convert.ToInt32(Info["trading_locks_count"]) : 0);//Trading lock counts
           WriteString(Convert.ToDouble(Info["trading_locked"]) != 0 ? (origin.ToString("dd/MM/yyyy HH:mm:ss")) : "0");//Trading lock
           WriteString("");//Purchases
            WriteInteger(0);//Itendity information tool
            WriteInteger(0);//Id bans.
           WriteString(User != null ? Convert.ToString(User["mail"]) : "Unknown");
           WriteString("");//user_classification
        }
    }
}
