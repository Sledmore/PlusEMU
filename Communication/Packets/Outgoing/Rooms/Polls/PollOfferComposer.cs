namespace Plus.Communication.Packets.Outgoing.Rooms.Polls
{
    class PollOfferComposer : MessageComposer
    {
        public PollOfferComposer()
            : base(1074)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(111141);//Room Id
            packet.WriteString("CLIENT_NPS");
            packet.WriteString("Customer Satisfaction Poll");
            packet.WriteString("Give us your opinion!");
        }
    }
}
