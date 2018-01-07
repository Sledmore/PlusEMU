using Plus.HabboHotel.Quests;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class RequestBuddyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            if (Session.GetHabbo().GetMessenger().RequestBuddy(Packet.PopString()))
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SocialFriend);
        }
    }
}
