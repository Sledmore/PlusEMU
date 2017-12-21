namespace Plus.HabboHotel.Quests
{
    public static class QuestTypeUtillity
    {
        public static string GetString(QuestType type)
        {
            switch (type)
            {
                case QuestType.SocialWave:
                    return "WAVE";

                case QuestType.SocialDance:
                    return "DANCE";

                case QuestType.SocialRespect:
                    return "GIVE_RESPECT";

                case QuestType.SocialFriend:
                    return "REQUEST_FRIEND";

                case QuestType.SocialChat:
                    return "CHAT_WITH_SOMEONE";

                case QuestType.SocialVisit:
                    return "ENTER_OTHERS_ROOM";

                case QuestType.ProfileBadge:
                    return "WEAR_BADGE";

                case QuestType.ProfileChangeMotto:
                    return "CHANGE_MOTTO";

                case QuestType.ProfileChangeLook:
                    return "CHANGE_FIGURE";

                case QuestType.FurniDecoWall:
                    return "PLACE_WALLPAPER";

                case QuestType.FurniDecoFloor:
                    return "PLACE_FLOOR";

                case QuestType.FurniStack:
                    return "STACK_ITEM";

                case QuestType.FurniSwitch:
                    return "SWITCH_ITEM_STATE";

                case QuestType.FurniPick:
                    return "PICKUP_ITEM";

                case QuestType.FurniPlace:
                    return "PLACE_ITEM";

                case QuestType.FurniRotate:
                    return "ROTATE_ITEM";

                case QuestType.FurniMove:
                    return "MOVE_ITEM";
                case QuestType.SummerEnterRoom:
                    return "ENTER_ROOM";

                case QuestType.AddFriends:
                    return "add_25_friends";
                case QuestType.WaveUsers:
                    return "wave_10_users";

                default:
                case QuestType.ExploreFindItem:
                    return "FIND_STUFF";
            }
        }
    }
}