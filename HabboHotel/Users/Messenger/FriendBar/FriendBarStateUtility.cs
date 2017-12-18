using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Users.Messenger.FriendBar
{
    public static class FriendBarStateUtility
    {
        public static FriendBarState GetEnum(int State)
        {
            switch (State)
            {
                default:
                case 0:
                    return FriendBarState.Closed;

                case 1:
                    return FriendBarState.Open;
            }
        }

        public static int GetInt(FriendBarState State)
        {
            switch (State)
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
