namespace Plus.HabboHotel.Navigator
{
    public static class NavigatorCategoryTypeUtility
    {
        public static NavigatorCategoryType GetCategoryTypeByString(string type)
        {
            switch (type.ToLower())
            {
                default:
                case "category":
                    return NavigatorCategoryType.Category;
                case "featured":
                    return NavigatorCategoryType.Featured;
                case "popular":
                    return NavigatorCategoryType.Popular;
                case "recommended":
                    return NavigatorCategoryType.Recommended;
                case "query":
                    return NavigatorCategoryType.Query;
                case "my_rooms":
                    return NavigatorCategoryType.MyRooms;
                case "my_favorites":
                    return NavigatorCategoryType.MyFavourites;
                case "my_groups":
                    return NavigatorCategoryType.MyGroups;
                case "my_history":
                    return NavigatorCategoryType.MyHistory;
                case "my_friends_room":
                    return NavigatorCategoryType.MyFriendsRooms;
                case "my_history_freq":
                    return NavigatorCategoryType.MyFrequentHistory;
                case "top_promotions":
                    return NavigatorCategoryType.TopPromotions;
                case "promotion_category":
                    return NavigatorCategoryType.PromotionCategory;
                case "my_rights":
                    return NavigatorCategoryType.MyRights;
            }
        }
    }
}
