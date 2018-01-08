using Plus.HabboHotel.Quests;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Quests;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Quests
{
    class StartQuestEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int questId = packet.PopInt();

            Quest quest = PlusEnvironment.GetGame().GetQuestManager().GetQuest(questId);
            if (quest == null)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("REPLACE INTO `user_quests` (`user_id`,`quest_id`) VALUES ('" + session.GetHabbo().Id + "', '" + quest.Id + "')");
                dbClient.RunQuery("UPDATE `user_stats` SET `quest_id` = '" + quest.Id + "' WHERE `id` = '" + session.GetHabbo().Id + "' LIMIT 1");
            }

            session.GetHabbo().GetStats().QuestId = quest.Id;
            PlusEnvironment.GetGame().GetQuestManager().GetList(session, null);
            session.SendPacket(new QuestStartedComposer(session, quest));
        }
    }
}
