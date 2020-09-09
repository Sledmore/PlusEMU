using Plus.HabboHotel.Quests;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Quests
{
    class QuestStartedComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public Quest Quest { get; }

        public QuestStartedComposer(GameClient Session, Quest Quest)
            : base(ServerPacketHeader.QuestStartedMessageComposer)
        {
            this.Habbo = Session.GetHabbo();
            this.Quest = Quest;
        }

        public override void Compose(ServerPacket packet)
        {
            QuestListComposer.SerializeQuest(packet, Habbo, Quest, Quest.Category);
        }
    }
}
