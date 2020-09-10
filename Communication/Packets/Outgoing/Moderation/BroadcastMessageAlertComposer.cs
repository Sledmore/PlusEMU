namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class BroadcastMessageAlertComposer : MessageComposer
    {
        public string Message { get; }
        public string Url { get; }
        public BroadcastMessageAlertComposer(string Message, string URL = "")
            : base(ServerPacketHeader.BroadcastMessageAlertMessageComposer)
        {
            this.Message = Message;
            this.Url = URL;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Message);
            packet.WriteString(Url);
        }
    }
}

