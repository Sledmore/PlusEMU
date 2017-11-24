using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Navigator;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    public class GetUserFlatCatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            ICollection<SearchResultList> Categories = PlusEnvironment.GetGame().GetNavigator().GetFlatCategories();

            Session.SendPacket(new UserFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}