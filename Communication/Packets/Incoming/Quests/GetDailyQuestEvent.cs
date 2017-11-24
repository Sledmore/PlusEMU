using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.LandingView;

namespace Plus.Communication.Packets.Incoming.Quests
{
    class GetDailyQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int UsersOnline = PlusEnvironment.GetGame().GetClientManager().Count;

            Session.SendPacket(new ConcurrentUsersGoalProgressComposer(UsersOnline));
        }
    }
}
