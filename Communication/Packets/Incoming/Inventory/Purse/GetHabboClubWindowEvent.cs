using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Incoming.Inventory.Purse
{
    class GetHabboClubWindowEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
           // Session.SendNotification("Habbo Club is free for all members, enjoy!");
        }
    }
}
