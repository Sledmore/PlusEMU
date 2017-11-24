using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Incoming;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class GetNavigatorFlatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ICollection<SearchResultList> Categories = PlusEnvironment.GetGame().GetNavigator().GetEventCategories();

            Session.SendPacket(new NavigatorFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}