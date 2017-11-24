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
                    return FriendBarState.CLOSED;

                case 1:
                    return FriendBarState.OPEN;
            }
        }

        public static int GetInt(FriendBarState State)
        {
            switch (State)
            {
                default:
                case FriendBarState.CLOSED:
                    return 0;

                case FriendBarState.OPEN:
                    return 1;
            }
        }
    }
}
