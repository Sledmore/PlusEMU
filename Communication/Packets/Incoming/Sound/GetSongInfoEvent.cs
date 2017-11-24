using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Sound;

namespace Plus.Communication.Packets.Incoming.Sound
{
    class GetSongInfoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new TraxSongInfoComposer());
        }
    }
}
