using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class NameChangeUpdateComposer : ServerPacket
    {
        public NameChangeUpdateComposer(string Name, int Error, ICollection<string> Tags)
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {
            base.WriteInteger(Error);
           base.WriteString(Name);

            base.WriteInteger(Tags.Count);
            foreach (string Tag in Tags)
            {
               base.WriteString(Name + Tag);
            }
        }

        public NameChangeUpdateComposer(string Name, int Error)
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {
            base.WriteInteger(Error);
           base.WriteString(Name);
            base.WriteInteger(0);
        }
    }
}
