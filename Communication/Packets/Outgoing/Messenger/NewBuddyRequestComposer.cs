using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Cache;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class NewBuddyRequestComposer : ServerPacket
    {
        public NewBuddyRequestComposer(UserCache Habbo)
            : base(ServerPacketHeader.NewBuddyRequestMessageComposer)
        {
            base.WriteInteger(Habbo.Id);
           base.WriteString(Habbo.Username);
           base.WriteString(Habbo.Look);
        }
    }
}
