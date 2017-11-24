using System.Collections.Generic;

using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Cache;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer(ICollection<MessengerRequest> requests)
            : base(ServerPacketHeader.BuddyRequestsMessageComposer)
        {
            base.WriteInteger(requests.Count);
            base.WriteInteger(requests.Count);

            foreach (MessengerRequest Request in requests)
            {
                base.WriteInteger(Request.From);
                base.WriteString(Request.Username);

                UserCache User = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Request.From);
                base.WriteString(User != null ? User.Look : "");
            }
        }
    }
}
