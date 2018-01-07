using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Quests
{
    public class GetQuestListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            PlusEnvironment.GetGame().GetQuestManager().GetList(Session, null);
        }
    }
}