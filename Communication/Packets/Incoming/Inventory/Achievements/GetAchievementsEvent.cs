using System.Linq;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;

namespace Plus.Communication.Packets.Incoming.Inventory.Achievements
{
    class GetAchievementsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new AchievementsComposer(Session, PlusEnvironment.GetGame().GetAchievementManager().Achievements.Values.ToList()));
        }
    }
}
