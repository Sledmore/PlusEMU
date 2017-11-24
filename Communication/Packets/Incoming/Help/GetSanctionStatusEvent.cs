using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Help;

namespace Plus.Communication.Packets.Incoming.Help
{
    class GetSanctionStatusEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            //Session.SendMessage(new SanctionStatusComposer());
        }
    }
}
