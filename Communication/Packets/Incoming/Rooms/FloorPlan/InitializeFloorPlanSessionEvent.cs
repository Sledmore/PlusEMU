using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Incoming.Rooms.FloorPlan
{
    class InitializeFloorPlanSessionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            //Session.SendNotif("WARNING - THIS TOOL IS IN BETA, IT COULD CORRUPT YOUR ROOM IF YOU CONFIGURE THE MAP WRONG OR DISCONNECT YOU.");
        }
    }
}
