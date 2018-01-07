using Plus.HabboHotel.Quests;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;


namespace Plus.Communication.Packets.Incoming.Inventory.Badges
{
    class SetActivatedBadgesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetBadgeComponent().ResetSlots();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_badges` SET `badge_slot` = '0' WHERE `user_id` = @userId");
                dbClient.AddParameter("userId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            for (int i = 0; i < 5; i++)
            {
                int Slot = Packet.PopInt();
                string Badge = Packet.PopString();

                if (Badge.Length == 0)
                    continue;

                if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Badge) || Slot < 1 || Slot > 5)
                    return;

                Session.GetHabbo().GetBadgeComponent().GetBadge(Badge).Slot = Slot;

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `user_badges` SET `badge_slot` = @slot WHERE `badge_id` = @badge AND `user_id` = @userId LIMIT 1");
                    dbClient.AddParameter("slot", Slot);
                    dbClient.AddParameter("badge", Badge);
                    dbClient.AddParameter("userId", Session.GetHabbo().Id);
                    dbClient.RunQuery();
                }
            }

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.ProfileBadge);

            Room Room;

            if (Session.GetHabbo().InRoom && PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                Session.GetHabbo().CurrentRoom.SendPacket(new HabboUserBadgesComposer(Session.GetHabbo()));
            else
                Session.SendPacket(new HabboUserBadgesComposer(Session.GetHabbo()));
        }
    }
}
