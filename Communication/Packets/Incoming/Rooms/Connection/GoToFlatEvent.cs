using System;
using System.Linq;
using System.Text;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.Communication.Packets.Incoming.Rooms.Connection
{
    class GoToFlatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            if (!Session.GetHabbo().EnterRoom(Session.GetHabbo().CurrentRoom))
                Session.SendPacket(new CloseConnectionComposer());
        }
    }
}
