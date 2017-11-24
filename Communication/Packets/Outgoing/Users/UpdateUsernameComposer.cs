using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class UpdateUsernameComposer : ServerPacket
    {
        public UpdateUsernameComposer(string User)
            : base(ServerPacketHeader.UpdateUsernameMessageComposer)
        {
            base.WriteInteger(0);
           base.WriteString(User);
            base.WriteInteger(0);
        }
    }
}