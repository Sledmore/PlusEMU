using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Misc;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class LatencyTestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            //Session.SendMessage(new LatencyTestComposer(Packet.PopInt()));
        }
    }
}
