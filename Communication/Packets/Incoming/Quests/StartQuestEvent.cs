using Plus.HabboHotel.Quests;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Quests;

namespace Plus.Communication.Packets.Incoming.Quests
{
    class StartQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int QuestId = Packet.PopInt();

            Quest Quest = PlusEnvironment.GetGame().GetQuestManager().GetQuest(QuestId);
            if (Quest == null)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("REPLACE INTO `user_quests` (`user_id`,`quest_id`) VALUES ('" + Session.GetHabbo().Id + "', '" + Quest.Id + "')");
                dbClient.RunQuery("UPDATE `user_stats` SET `quest_id` = '" + Quest.Id + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            Session.GetHabbo().GetStats().QuestID = Quest.Id;
            PlusEnvironment.GetGame().GetQuestManager().GetList(Session, null);
            Session.SendPacket(new QuestStartedComposer(Session, Quest));
        }
    }
}
