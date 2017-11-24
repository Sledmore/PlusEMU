using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Quests;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Quests
{
    class QuestStartedComposer : ServerPacket
    {
        public QuestStartedComposer(GameClient Session, Quest Quest)
            : base(ServerPacketHeader.QuestStartedMessageComposer)
        {
            this.SerializeQuest(this, Session, Quest, Quest.Category);
        }

        private void SerializeQuest(ServerPacket Message, GameClient Session, Quest Quest, string Category)
        {
            if (Message == null || Session == null)
                return;

            int AmountInCat = PlusEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Category);
            int Number = Quest == null ? AmountInCat : Quest.Number - 1;
            int UserProgress = Quest == null ? 0 : Session.GetHabbo().GetQuestProgress(Quest.Id);

            if (Quest != null && Quest.IsCompleted(UserProgress))
                Number++;

            Message.WriteString(Category);
            Message.WriteInteger(Quest == null ? 0 : ((Quest.Category.Contains("xmas2012")) ? 0 : Number));  // Quest progress in this cat
            Message.WriteInteger(Quest == null ? 0 : (Quest.Category.Contains("xmas2012")) ? 0 : AmountInCat); // Total quests in this cat
            Message.WriteInteger(Quest == null ? 3 : Quest.RewardType);// Reward type (1 = Snowflakes, 2 = Love hearts, 3 = Pixels, 4 = Seashells, everything else is pixels
            Message.WriteInteger(Quest == null ? 0 : Quest.Id); // Quest id
            Message.WriteBoolean(Quest == null ? false : Session.GetHabbo().GetStats().QuestID == Quest.Id);  // Quest started
            Message.WriteString(Quest == null ? string.Empty : Quest.ActionName);
            Message.WriteString(Quest == null ? string.Empty : Quest.DataBit);
            Message.WriteInteger(Quest == null ? 0 : Quest.Reward);
            Message.WriteString(Quest == null ? string.Empty : Quest.Name);
            Message.WriteInteger(UserProgress); // Current progress
            Message.WriteInteger(Quest == null ? 0 : Quest.GoalData); // Target progress
            Message.WriteInteger(Quest == null ? 0 : Quest.TimeUnlock); // "Next quest available countdown" in seconds
            Message.WriteString("");
            Message.WriteString("");
            Message.WriteBoolean(true);
        }
    }
}
