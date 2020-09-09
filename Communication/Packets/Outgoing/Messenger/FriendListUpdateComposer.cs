using System;
using System.Linq;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Relationships;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class FriendListUpdateComposer : MessageComposer
    {
        public int FriendId { get; }
        
        public Habbo Habbo { get; }
        public MessengerBuddy Buddy { get; }

        public FriendListUpdateComposer(int FriendId)
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
            this.FriendId = FriendId;
        }

        public FriendListUpdateComposer(Habbo habbo, MessengerBuddy Buddy)
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
            this.Habbo = habbo;
            this.Buddy = Buddy;
        }

        public override void Compose(ServerPacket packet)
        {
            if(this.Habbo != null)
            {
                packet.WriteInteger(0);//Category Count
                packet.WriteInteger(1);//Updates Count
                packet.WriteInteger(0);//Update

                Buddy.Serialize(packet, Habbo);
            } else
            {
                packet.WriteInteger(0);//Category Count
                packet.WriteInteger(1);//Updates Count
                packet.WriteInteger(-1);//Update
                packet.WriteInteger(FriendId);
            }
        }
    }
}
