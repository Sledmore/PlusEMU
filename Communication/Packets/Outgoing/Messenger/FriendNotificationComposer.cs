using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class FriendNotificationComposer : MessageComposer
    {
        public int UserId { get; }
        public MessengerEventTypes Type { get; }
        public string Data { get; }

        public FriendNotificationComposer(int UserId, MessengerEventTypes type, string data)
            : base(ServerPacketHeader.FriendNotificationMessageComposer)
        {
            this.UserId = UserId;
            this.Type = type;
            this.Data = data;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(UserId.ToString());
            packet.WriteInteger(MessengerEventTypesUtility.GetEventTypePacketNum(Type));
            packet.WriteString(Data);
        }
    }
}
