using System.Collections.Generic;
using System.Linq;
using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Outgoing.Navigator.New
{
    class NavigatorMetaDataParserComposer : MessageComposer
    {
        public ICollection<TopLevelItem> TopLevelItems { get; }

        public NavigatorMetaDataParserComposer(ICollection<TopLevelItem> topLevelItems)
            : base(ServerPacketHeader.NavigatorMetaDataParserMessageComposer)
        {
            this.TopLevelItems = topLevelItems;
        }

        public override void Compose(ServerPacket packet)
        {
            //TODO: HMU here too, if you want saved searches to be fixed
            packet.WriteInteger(TopLevelItems.Count); //Count
            foreach (TopLevelItem topLevelItem in TopLevelItems.ToList())
            {
                //TopLevelContext
                packet.WriteString(topLevelItem.SearchCode); //Search code
                packet.WriteInteger(0); //Count of saved searches?
                /*{
                    //SavedSearch
                   packet.WriteInteger(TopLevelItem.Id);//Id
                   packet.WriteString(TopLevelItem.SearchCode);//Search code
                   packet.WriteString(TopLevelItem.Filter);//Filter
                   packet.WriteString(TopLevelItem.Localization);//localization
                }*/
            }
        }
    }
}
