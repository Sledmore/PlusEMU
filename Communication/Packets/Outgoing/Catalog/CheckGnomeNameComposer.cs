using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class CheckGnomeNameComposer : ServerPacket
    {
        public CheckGnomeNameComposer(string PetName, int ErrorId)
            : base(ServerPacketHeader.CheckGnomeNameMessageComposer)
        {
            base.WriteInteger(0);
            base.WriteInteger(ErrorId);
           base.WriteString(PetName);
        }
    }
}
