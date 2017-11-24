using Plus.HabboHotel.Quests;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Quests;

namespace Plus.Communication.Packets.Incoming.Quests
{
    class CancelQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Quest Quest = PlusEnvironment.GetGame().GetQuestManager().GetQuest(Session.GetHabbo().GetStats().QuestID);
            if (Quest == null)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `user_quests` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `quest_id` = '" + Quest.Id + "';" +
                    "UPDATE `user_stats` SET `quest_id` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            Session.GetHabbo().GetStats().QuestID = 0;
            Session.SendPacket(new QuestAbortedComposer());

            PlusEnvironment.GetGame().GetQuestManager().GetList(Session, null);
        }
    }
}
