namespace Plus.Communication.Packets.Outgoing.Rooms.Notifications
{
    class RoomNotificationComposer : MessageComposer
    {
        public string Type { get; }
        public string Key { get; }
        public string Value { get; }

        public string Title { get; }
        public string Message { get; }
        public string Image { get; }
        public string HotelName { get; }
        public string HotelUrl { get; }
        public RoomNotificationComposer(string Type, string Key, string Value)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            this.Type = Type;
            this.Key = Key;
            this.Value = Value;
        }

        public RoomNotificationComposer(string Type)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            this.Type = Type;
        }

        public RoomNotificationComposer(string Title, string Message, string Image, string HotelName = "", string HotelURL = "")
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
            this.Title = Title;
            this.Message = Message;
            this.Image = Image;
            this.HotelName = HotelName;
            this.HotelUrl = HotelURL;
        }

        public override void Compose(ServerPacket packet)
        {
            if(!string.IsNullOrEmpty(Message))
            {
                packet.WriteString(Image);
                packet.WriteInteger(string.IsNullOrEmpty(HotelName) ? 2 : 4);
                packet.WriteString("title");
                packet.WriteString(Title);
                packet.WriteString("message");
                packet.WriteString(Message);

                if (!string.IsNullOrEmpty(HotelName))
                {
                    packet.WriteString("linkUrl");
                    packet.WriteString(HotelUrl);
                    packet.WriteString("linkTitle");
                    packet.WriteString(HotelName);
                }
            } else if(!string.IsNullOrEmpty(Key))
            {
                packet.WriteString(Type);
                packet.WriteInteger(1);//Count
                {
                    packet.WriteString(Key);//Type of message
                    packet.WriteString(Value);
                }
            } else
            {
                packet.WriteString(Type);
                packet.WriteInteger(0);//Count
            }
        }
    }
}
