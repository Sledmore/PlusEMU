using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Inventory.Achievements;

namespace Plus.Communication.Packets.Incoming.Inventory.Achievements
{
    class GetAchievementsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new AchievementsComposer(Session, PlusEnvironment.GetGame().GetAchievementManager()._achievements.Values.ToList()));
        }
    }
}
