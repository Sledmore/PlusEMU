using Plus.Database.Interfaces;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerationBanEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_soft_ban"))
                return;

            int userId = packet.PopInt();
            string message = packet.PopString();
            double length = packet.PopInt() * 3600 + PlusEnvironment.GetUnixTimestamp();
            packet.PopString(); //unk1
            packet.PopString(); //unk2
            bool ipBan = packet.PopBoolean();
            bool machineBan = packet.PopBoolean();

            if (machineBan)
                ipBan = false;

            Habbo habbo = PlusEnvironment.GetHabboById(userId);

            if (habbo == null)
            {
                session.SendWhisper("An error occoured whilst finding that user in the database.");
                return;
            }

            if (habbo.GetPermissions().HasRight("mod_tool") && !session.GetHabbo().GetPermissions().HasRight("mod_ban_any"))
            {
                session.SendWhisper("Oops, you cannot ban that user.");
                return;
            }

            message = message != null ? message : "No reason specified.";

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + habbo.Id + "' LIMIT 1");
            }

            if (ipBan == false && machineBan == false)
                PlusEnvironment.GetGame().GetModerationManager().BanUser(session.GetHabbo().Username, ModerationBanType.Username, habbo.Username, message, length);
            else if (ipBan)
                PlusEnvironment.GetGame().GetModerationManager().BanUser(session.GetHabbo().Username, ModerationBanType.IP, habbo.Username, message, length);
            else
            {
                PlusEnvironment.GetGame().GetModerationManager().BanUser(session.GetHabbo().Username, ModerationBanType.IP, habbo.Username, message, length);
                PlusEnvironment.GetGame().GetModerationManager().BanUser(session.GetHabbo().Username, ModerationBanType.Username, habbo.Username, message, length);
                PlusEnvironment.GetGame().GetModerationManager().BanUser(session.GetHabbo().Username, ModerationBanType.Machine, habbo.Username, message, length);
            }

            GameClient targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(habbo.Username);
            if (targetClient != null)
                targetClient.Disconnect();
        }
    }
}