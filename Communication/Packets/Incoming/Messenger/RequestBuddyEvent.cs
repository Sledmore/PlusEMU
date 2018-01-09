using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class RequestBuddyEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null)
                return;

            if (session.GetHabbo().GetMessenger().RequestBuddy(packet.PopString()))
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialFriend);
        }
    }
}
