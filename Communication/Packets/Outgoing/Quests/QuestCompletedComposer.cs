using Plus.HabboHotel.Quests;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Quests
{
    class QuestCompletedComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public Quest Quest { get; }

        public QuestCompletedComposer(GameClient Session, Quest Quest)
            : base(ServerPacketHeader.QuestCompletedMessageComposer)
        {
            this.Habbo = Session.GetHabbo();
            this.Quest = Quest;
        }

        public override void Compose(ServerPacket packet)
        {
            int AmountInCat = PlusEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Quest.Category);
            int Number = Quest == null ? AmountInCat : Quest.Number;
            int UserProgress = Quest == null ? 0 : Habbo.GetQuestProgress(Quest.Id);

            packet.WriteString(Quest.Category);
            packet.WriteInteger(Number); // Quest progress in this cat
            packet.WriteInteger((Quest.Name.Contains("xmas2012")) ? 1 : AmountInCat); // Total quests in this cat
            packet.WriteInteger(Quest == null ? 3 : Quest.RewardType); // Reward type (1 = Snowflakes, 2 = Love hearts, 3 = Pixels, 4 = Seashells, everything else is pixels
            packet.WriteInteger(Quest == null ? 0 : Quest.Id); // Quest id
            packet.WriteBoolean(Quest == null ? false : Habbo.GetStats().QuestId == Quest.Id); // Quest started
            packet.WriteString(Quest == null ? string.Empty : Quest.ActionName);
            packet.WriteString(Quest == null ? string.Empty : Quest.DataBit);
            packet.WriteInteger(Quest == null ? 0 : Quest.Reward);
            packet.WriteString(Quest == null ? string.Empty : Quest.Name);
            packet.WriteInteger(UserProgress); // Current progress
            packet.WriteInteger(Quest == null ? 0 : Quest.GoalData); // Target progress
            packet.WriteInteger(Quest == null ? 0 : Quest.TimeUnlock); // "Next quest available countdown" in seconds
            packet.WriteString("");
            packet.WriteString("");
            packet.WriteBoolean(true); // ?
            packet.WriteBoolean(true); // Activate next quest..
        }
    }
}
