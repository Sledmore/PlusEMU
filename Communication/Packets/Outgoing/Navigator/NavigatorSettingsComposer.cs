using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int Homeroom)
            : base(ServerPacketHeader.NavigatorSettingsMessageComposer)
        {
            base.WriteInteger(Homeroom);
            base.WriteInteger(Homeroom);
        }
    }
}
