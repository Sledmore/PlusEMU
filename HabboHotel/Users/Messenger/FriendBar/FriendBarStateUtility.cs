namespace Plus.HabboHotel.Users.Messenger.FriendBar
{
    public static class FriendBarStateUtility
    {
        public static FriendBarState GetEnum(int state)
        {
            switch (state)
            {
                default:
                case 0:
                    return FriendBarState.Closed;

                case 1:
                    return FriendBarState.Open;
            }
        }

        public static int GetInt(FriendBarState state)
        {
            switch (state)
            {
                default:
                case FriendBarState.Closed:
                    return 0;

                case FriendBarState.Open:
                    return 1;
            }
        }
    }
}
