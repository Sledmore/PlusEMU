using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class ClientVariablesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string GordanPath = Packet.PopString();
            string ExternalVariables = Packet.PopString();
        }
    }
}
