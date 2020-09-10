using System.Linq;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Achievements
{
    class GetAchievementsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new AchievementsComposer(session.GetHabbo(), PlusEnvironment.GetGame().GetAchievementManager().Achievements.Values.ToList()));
        }
    }
}
