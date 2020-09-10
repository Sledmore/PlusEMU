namespace Plus.Communication.Packets.Outgoing.LandingView
{
    class ConcurrentUsersGoalProgressComposer : MessageComposer
    {
        public int UsersNow { get; }

        public ConcurrentUsersGoalProgressComposer(int UsersNow) 
            : base(ServerPacketHeader.ConcurrentUsersGoalProgressMessageComposer)
        {
            this.UsersNow = UsersNow;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);//0/1 = Not done, 2 = Done & can claim, 3 = claimed.
            packet.WriteInteger(UsersNow);
            packet.WriteInteger(1000);
        }
    }
}
