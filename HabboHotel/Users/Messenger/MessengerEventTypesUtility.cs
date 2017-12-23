namespace Plus.HabboHotel.Users.Messenger
{
   public static class MessengerEventTypesUtility
    {
        public static int GetEventTypePacketNum(MessengerEventTypes type)
        {
            switch (type)
            {
                case MessengerEventTypes.EventStarted:
                    return 0;

                case MessengerEventTypes.AchievementUnlocked:
                    return 1;

                case MessengerEventTypes.QuestCompleted:
                    return 2;

                case MessengerEventTypes.PlayingGame:
                    return 3;

                case MessengerEventTypes.FinishedGame:
                    return 4;

                case MessengerEventTypes.GameInvite:
                    return 5;

                default:
                    return -1;
            }
        }
    }
}
