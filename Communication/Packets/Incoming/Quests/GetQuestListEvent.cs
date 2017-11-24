using System.Collections.Generic;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.Communication.Packets.Incoming;

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