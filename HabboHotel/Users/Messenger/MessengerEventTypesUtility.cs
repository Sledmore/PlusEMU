using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Users.Messenger
{
   public static class MessengerEventTypesUtility
    {
        public static int GetEventTypePacketNum(MessengerEventTypes type)
        {
            switch (type)
            {
                case MessengerEventTypes.EVENT_STARTED:
                    return 0;

                case MessengerEventTypes.ACHIEVEMENT_UNLOCKED:
                    return 1;

                case MessengerEventTypes.QUEST_COMPLETED:
                    return 2;

                case MessengerEventTypes.PLAYING_GAME:
                    return 3;

                case MessengerEventTypes.FINISHED_GAME:
                    return 4;

                case MessengerEventTypes.GAME_INVITE:
                    return 5;

                default:
                    return -1;
            }
        }
    }
}
