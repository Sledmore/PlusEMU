using Plus.HabboHotel.Quests;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;


namespace Plus.Communication.Packets.Incoming.Inventory.Badges
{
    class SetActivatedBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.GetHabbo().GetBadgeComponent().ResetSlots();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_badges` SET `badge_slot` = '0' WHERE `user_id` = @userId");
                dbClient.AddParameter("userId", session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            for (int i = 0; i < 5; i++)
            {
                int slot = packet.PopInt();
                string badge = packet.PopString();

                if (badge.Length == 0)
                    continue;

                if (!session.GetHabbo().GetBadgeComponent().HasBadge(badge) || slot < 1 || slot > 5)
                    return;

                session.GetHabbo().GetBadgeComponent().GetBadge(badge).Slot = slot;

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `user_badges` SET `badge_slot` = @slot WHERE `badge_id` = @badge AND `user_id` = @userId LIMIT 1");
                    dbClient.AddParameter("slot", slot);
                    dbClient.AddParameter("badge", badge);
                    dbClient.AddParameter("userId", session.GetHabbo().Id);
                    dbClient.RunQuery();
                }
            }

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ProfileBadge);


            if (session.GetHabbo().InRoom && PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                room.SendPacket(new HabboUserBadgesComposer(session.GetHabbo()));
            else
                session.SendPacket(new HabboUserBadgesComposer(session.GetHabbo()));
        }
    }
}
