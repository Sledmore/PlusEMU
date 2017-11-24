using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Navigator.New;
using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class InitializeNewNavigatorEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<TopLevelItem> TopLevelItems = PlusEnvironment.GetGame().GetNavigator().GetTopLevelItems();

            Session.SendPacket(new NavigatorMetaDataParserComposer(TopLevelItems));
            Session.SendPacket(new NavigatorLiftedRoomsComposer());
            Session.SendPacket(new NavigatorCollapsedCategoriesComposer());
            Session.SendPacket(new NavigatorPreferencesComposer());
        }
    }
}
