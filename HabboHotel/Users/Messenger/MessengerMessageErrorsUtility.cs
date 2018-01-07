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
                case MessengerMessageErrors.FriendMuted:
                    return 3;

                case MessengerMessageErrors.yourMuted:
                    return 4;

                case MessengerMessageErrors.FriendOffline:
                    return 5;

                case MessengerMessageErrors.NotFriends:
                    return 6;

                case MessengerMessageErrors.FriendBusy:
                    return 7;

                case MessengerMessageErrors.OfflineFailed:
                    return 10;
            }
        }
    }
}
