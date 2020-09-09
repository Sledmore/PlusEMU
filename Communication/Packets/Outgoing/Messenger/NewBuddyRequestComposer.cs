using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class NewBuddyRequestComposer : MessageComposer
    {
        public UserCache UserCache { get; }

        public NewBuddyRequestComposer(UserCache Habbo)
            : base(ServerPacketHeader.NewBuddyRequestMessageComposer)
        {
            this.UserCache = Habbo;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserCache.Id);
            packet.WriteString(UserCache.Username);
            packet.WriteString(UserCache.Look);
        }
    }
}
