namespace Plus.Communication.Packets.Outgoing.Rooms.Notifications
{
    class RoomNotificationComposer : ServerPacket
    {
        public RoomNotificationComposer(string Type, string Key, string Value)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            WriteString(Type);
            WriteInteger(1);//Count
            {
                WriteString(Key);//Type of message
                WriteString(Value);
            }
        }

        public RoomNotificationComposer(string Type)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            WriteString(Type);
            WriteInteger(0);//Count
        }

        public RoomNotificationComposer(string Title, string Message, string Image, string HotelName = "", string HotelURL = "")
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            WriteString(Image);
            WriteInteger(string.IsNullOrEmpty(HotelName) ? 2 : 4);
            WriteString("title");
            WriteString(Title);
            WriteString("message");
            WriteString(Message);

            if (!string.IsNullOrEmpty(HotelName))
            {
                WriteString("linkUrl");
                WriteString(HotelURL);
                WriteString("linkTitle");
                WriteString(HotelName);
            }
        }
    }
}
