using Plus.Database.Interfaces;
using Plus.HabboHotel.Users;


namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerationTradeLockEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_trade_lock"))
                return;

            int userId = packet.PopInt();
            string message = packet.PopString();
            double days = packet.PopInt() / 1440.0;
            packet.PopString(); //unk1
            packet.PopString(); //unk2

            double length = PlusEnvironment.GetUnixTimestamp() + days * 86400;

            Habbo habbo = PlusEnvironment.GetHabboById(userId);
            if (habbo == null)
            {
                session.SendWhisper("An error occoured whilst finding that user in the database.");
                return;
            }

            if (habbo.GetPermissions().HasRight("mod_trade_lock") && !session.GetHabbo().GetPermissions().HasRight("mod_trade_lock_any"))
            {
                session.SendWhisper("Oops, you cannot trade lock another user ranked 5 or higher.");
                return;
            }

            if (days < 1)
                days = 1;

            if (days > 365)
                days = 365;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '" + length + "', `trading_locks_count` = `trading_locks_count` + '1' WHERE `user_id` = '" + habbo.Id + "' LIMIT 1");
            }

            if (habbo.GetClient() != null)
            {
                habbo.TradingLockExpiry = length;
                habbo.GetClient().SendNotification("You have been trade banned for " + days + " day(s)!\r\rReason:\r\r" + message);
            }
        }
    }
}
