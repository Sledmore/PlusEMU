namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class NewConsoleMessageComposer : MessageComposer
    {
        public int Sender { get; }
        public string Message { get; }
        public int Time { get; }

        public NewConsoleMessageComposer(int Sender, string Message, int Time = 0)
            : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
            this.Sender = Sender;
            this.Message = Message;
            this.Time = Time;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Sender);
            packet.WriteString(Message);
            packet.WriteInteger(Time);
        }
    }
}
