using System.Data;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorUserInfoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int UserId = Packet.PopInt();

            DataRow User = null;
            DataRow Info = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`online`,`mail`,`ip_last`,`look`,`account_created`,`last_online` FROM `users` WHERE `id` = '" + UserId + "' LIMIT 1");
                User = dbClient.GetRow();

                if (User == null)
                {
                    Session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("user.not_found"));
                    return;
                }

                dbClient.SetQuery("SELECT `cfhs`,`cfhs_abusive`,`cautions`,`bans`,`trading_locked`,`trading_locks_count` FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                Info = dbClient.GetRow();
                if (Info == null)
                {
                    dbClient.RunQuery("INSERT INTO `user_info` (`user_id`) VALUES ('" + UserId + "')");
                    dbClient.SetQuery("SELECT `cfhs`,`cfhs_abusive`,`cautions`,`bans`,`trading_locked`,`trading_locks_count` FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                    Info = dbClient.GetRow();
                }
            }


            Session.SendPacket(new ModeratorUserInfoComposer(User, Info));
        }
    }
}