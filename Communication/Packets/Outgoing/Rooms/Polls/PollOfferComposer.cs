namespace Plus.Communication.Packets.Outgoing.Rooms.Polls
{
    class PollOfferComposer : ServerPacket
    {
        public PollOfferComposer()
            : base(1074)
        {
            base.WriteInteger(111141);//Room Id
           base.WriteString("CLIENT_NPS");
           base.WriteString("Customer Satisfaction Poll");
           base.WriteString("Give us your opinion!");
        }
    }
}
