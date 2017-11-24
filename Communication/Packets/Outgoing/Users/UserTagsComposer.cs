using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class UserTagsComposer : ServerPacket
    {
        public UserTagsComposer(int UserId)
            : base(ServerPacketHeader.UserTagsMessageComposer)
        {
            base.WriteInteger(UserId);
            base.WriteInteger(0);//Count of the tags.
            {
                //Append a string.
            }
        }
    }
}
