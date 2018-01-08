using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Navigator.New;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class InitializeNewNavigatorEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            ICollection<TopLevelItem> topLevelItems = PlusEnvironment.GetGame().GetNavigator().GetTopLevelItems();

            session.SendPacket(new NavigatorMetaDataParserComposer(topLevelItems));
            session.SendPacket(new NavigatorLiftedRoomsComposer());
            session.SendPacket(new NavigatorCollapsedCategoriesComposer());
            session.SendPacket(new NavigatorPreferencesComposer());
        }
    }
}
