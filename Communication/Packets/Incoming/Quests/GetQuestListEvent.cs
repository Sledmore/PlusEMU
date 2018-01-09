using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Quests
{
    public class GetQuestListEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            PlusEnvironment.GetGame().GetQuestManager().GetList(session, null);
        }
    }
}