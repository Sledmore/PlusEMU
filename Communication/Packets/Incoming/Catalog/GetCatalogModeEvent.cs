using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Catalog;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.BuildersClub;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class GetCatalogModeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string PageMode = Packet.PopString();
        }
    }
}
