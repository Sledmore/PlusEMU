using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class GetRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int Something = Packet.PopInt();
            Session.SendPacket(new RentableSpaceComposer());
        }
    }
}
