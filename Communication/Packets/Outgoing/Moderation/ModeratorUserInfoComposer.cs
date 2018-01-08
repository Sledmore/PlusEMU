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


            base.WriteInteger(User != null ? Convert.ToInt32(User["id"]) : 0);
           base.WriteString(User != null ? Convert.ToString(User["username"]) : "Unknown");
           base.WriteString(User != null ? Convert.ToString(User["look"]) : "Unknown");
            base.WriteInteger(User != null ? Convert.ToInt32(Math.Ceiling((PlusEnvironment.GetUnixTimestamp() - Convert.ToDouble(User["account_created"])) / 60)) : 0);
            base.WriteInteger(User != null ? Convert.ToInt32(Math.Ceiling((PlusEnvironment.GetUnixTimestamp() - Convert.ToDouble(User["last_online"])) / 60)) : 0);
            base.WriteBoolean(User != null ? PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Convert.ToInt32(User["id"])) != null : false);
            base.WriteInteger(Info != null ? Convert.ToInt32(Info["cfhs"]) : 0);
            base.WriteInteger(Info != null ? Convert.ToInt32(Info["cfhs_abusive"]) : 0);
            base.WriteInteger(Info != null ? Convert.ToInt32(Info["cautions"]) : 0);
            base.WriteInteger(Info != null ? Convert.ToInt32(Info["bans"]) : 0);
            base.WriteInteger(Info != null ? Convert.ToInt32(Info["trading_locks_count"]) : 0);//Trading lock counts
           base.WriteString(Convert.ToDouble(Info["trading_locked"]) != 0 ? (origin.ToString("dd/MM/yyyy HH:mm:ss")) : "0");//Trading lock
           base.WriteString("");//Purchases
            base.WriteInteger(0);//Itendity information tool
            base.WriteInteger(0);//Id bans.
           base.WriteString(User != null ? Convert.ToString(User["mail"]) : "Unknown");
           base.WriteString("");//user_classification
        }
    }
}
