namespace Plus.Communication.Packets.Outgoing.Rooms.Notifications
{
    class RoomNotificationComposer : ServerPacket
    {
        public RoomNotificationComposer(string Type, string Key, string Value)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            base.WriteString(Type);
            base.WriteInteger(1);//Count
            {
                base.WriteString(Key);//Type of message
                base.WriteString(Value);
            }
        }

        public RoomNotificationComposer(string Type)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            base.WriteString(Type);
            base.WriteInteger(0);//Count
        }

        public RoomNotificationComposer(string Title, string Message, string Image, string HotelName = "", string HotelURL = "")
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            base.WriteString(Image);
            base.WriteInteger(string.IsNullOrEmpty(HotelName) ? 2 : 4);
            base.WriteString("title");
            base.WriteString(Title);
            base.WriteString("message");
            base.WriteString(Message);

            if (!string.IsNullOrEmpty(HotelName))
            {
                base.WriteString("linkUrl");
                base.WriteString(HotelURL);
                base.WriteString("linkTitle");
                base.WriteString(HotelName);
            }
        }
    }
}
