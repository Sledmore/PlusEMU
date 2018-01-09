namespace Plus.Communication.Packets.Outgoing.LandingView
{
    class ConcurrentUsersGoalProgressComposer : ServerPacket
    {
        public ConcurrentUsersGoalProgressComposer(int UsersNow) 
            : base(ServerPacketHeader.ConcurrentUsersGoalProgressMessageComposer)
        {
            WriteInteger(0);//0/1 = Not done, 2 = Done & can claim, 3 = claimed.
            WriteInteger(UsersNow);
            WriteInteger(1000);
        }
    }
}
