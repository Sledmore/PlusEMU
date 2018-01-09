using System.Collections.Generic;

using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer(ICollection<MessengerRequest> requests)
            : base(ServerPacketHeader.BuddyRequestsMessageComposer)
        {
            WriteInteger(requests.Count);
            WriteInteger(requests.Count);

            foreach (MessengerRequest Request in requests)
            {
                WriteInteger(Request.From);
                WriteString(Request.Username);

                UserCache User = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Request.From);
                WriteString(User != null ? User.Look : "");
            }
        }
    }
}
