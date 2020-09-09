using System.Collections.Generic;

using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class BuddyRequestsComposer : MessageComposer
    {
        public ICollection<MessengerRequest> Requests { get; }

        public BuddyRequestsComposer(ICollection<MessengerRequest> requests)
            : base(ServerPacketHeader.BuddyRequestsMessageComposer)
        {
            this.Requests = requests;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Requests.Count);
            packet.WriteInteger(Requests.Count);

            foreach (MessengerRequest Request in Requests)
            {
                packet.WriteInteger(Request.From);
                packet.WriteString(Request.Username);

                UserCache User = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Request.From);
                packet.WriteString(User != null ? User.Look : "");
            }
        }
    }
}
