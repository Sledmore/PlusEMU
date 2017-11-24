using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.HabboHotel.Users.Messenger
{
    public static class MessengerMessageErrorsUtility
    {
        public static int GetMessageErrorPacketNum(MessengerMessageErrors error)
        {
          /*_-1xC[3] = "${messenger.error.receivermuted}";
            _-1xC[4] = "${messenger.error.sendermuted}";
            _-1xC[5] = "${messenger.error.offline}";
            _-1xC[6] = "${messenger.error.notfriend}";
            _-1xC[7] = "${messenger.error.busy}";
            _-1xC[8] = "${messenger.error.receiverhasnochat}";
            _-1xC[9] = "${messenger.error.senderhasnochat}";
            _-1xC[10] = "${messenger.error.offline_failed}";*/

            switch (error)
            {
                default:
                case MessengerMessageErrors.FRIEND_MUTED:
                    return 3;

                case MessengerMessageErrors.YOUR_MUTED:
                    return 4;

                case MessengerMessageErrors.FRIEND_NOT_ONLINE:
                    return 5;

                case MessengerMessageErrors.YOUR_NOT_FRIENDS:
                    return 6;

                case MessengerMessageErrors.FRIEND_BUSY:
                    return 7;

                case MessengerMessageErrors.OFFLINE_FAILED:
                    return 10;
            }
        }
    }
}
