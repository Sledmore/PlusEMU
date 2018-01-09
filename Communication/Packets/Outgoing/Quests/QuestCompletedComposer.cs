using Plus.HabboHotel.Quests;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Quests
{
    class QuestCompletedComposer : ServerPacket
    {
        public QuestCompletedComposer(GameClient Session, Quest Quest)
            : base(ServerPacketHeader.QuestCompletedMessageComposer)
        {
            int AmountInCat = PlusEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Quest.Category);
            int Number = Quest == null ? AmountInCat : Quest.Number;
            int UserProgress = Quest == null ? 0 : Session.GetHabbo().GetQuestProgress(Quest.Id);

           WriteString(Quest.Category);
            WriteInteger(Number); // Quest progress in this cat
            WriteInteger((Quest.Name.Contains("xmas2012")) ? 1 : AmountInCat); // Total quests in this cat
            WriteInteger(Quest == null ? 3 : Quest.RewardType); // Reward type (1 = Snowflakes, 2 = Love hearts, 3 = Pixels, 4 = Seashells, everything else is pixels
            WriteInteger(Quest == null ? 0 : Quest.Id); // Quest id
            WriteBoolean(Quest == null ? false : Session.GetHabbo().GetStats().QuestId == Quest.Id); // Quest started
           WriteString(Quest == null ? string.Empty : Quest.ActionName);
           WriteString(Quest == null ? string.Empty : Quest.DataBit);
            WriteInteger(Quest == null ? 0 : Quest.Reward);
           WriteString(Quest == null ? string.Empty : Quest.Name);
            WriteInteger(UserProgress); // Current progress
            WriteInteger(Quest == null ? 0 : Quest.GoalData); // Target progress
            WriteInteger(Quest == null ? 0 : Quest.TimeUnlock); // "Next quest available countdown" in seconds
           WriteString("");
           WriteString("");
            WriteBoolean(true); // ?
            WriteBoolean(true); // Activate next quest..
        }
    }
}
