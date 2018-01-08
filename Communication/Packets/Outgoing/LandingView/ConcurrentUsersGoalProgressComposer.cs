namespace Plus.Communication.Packets.Outgoing.LandingView
{
    class ConcurrentUsersGoalProgressComposer : ServerPacket
    {
        public ConcurrentUsersGoalProgressComposer(int UsersNow) 
            : base(ServerPacketHeader.ConcurrentUsersGoalProgressMessageComposer)
        {
            base.WriteInteger(0);//0/1 = Not done, 2 = Done & can claim, 3 = claimed.
            base.WriteInteger(UsersNow);
            base.WriteInteger(1000);
        }
    }
}
