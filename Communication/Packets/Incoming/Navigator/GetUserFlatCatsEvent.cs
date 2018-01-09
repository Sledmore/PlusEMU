using System.Collections.Generic;
using Plus.HabboHotel.Navigator;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    public class GetUserFlatCatsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            ICollection<SearchResultList> categories = PlusEnvironment.GetGame().GetNavigator().GetFlatCategories();

            session.SendPacket(new UserFlatCatsComposer(categories, session.GetHabbo().Rank));
        }
    }
}