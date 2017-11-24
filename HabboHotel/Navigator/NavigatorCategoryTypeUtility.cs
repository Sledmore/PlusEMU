using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Navigator
{
    public static class NavigatorCategoryTypeUtility
    {
        public static NavigatorCategoryType GetCategoryTypeByString(string CategoryType)
        {
            switch (CategoryType.ToLower())
            {
                default:
                case "category":
                    return NavigatorCategoryType.CATEGORY;
                case "featured":
                    return NavigatorCategoryType.FEATURED;
                case "popular":
                    return NavigatorCategoryType.POPULAR;
                case "recommended":
                    return NavigatorCategoryType.RECOMMENDED;
                case "query":
                    return NavigatorCategoryType.QUERY;
                case "my_rooms":
                    return NavigatorCategoryType.MY_ROOMS;
                case "my_favorites":
                    return NavigatorCategoryType.MY_FAVORITES;
                case "my_groups":
                    return NavigatorCategoryType.MY_GROUPS;
                case "my_history":
                    return NavigatorCategoryType.MY_HISTORY;
                case "my_friends_room":
                    return NavigatorCategoryType.MY_FRIENDS_ROOMS;
                case "my_history_freq":
                    return NavigatorCategoryType.MY_HISTORY_FREQ;
                case "top_promotions":
                    return NavigatorCategoryType.TOP_PROMOTIONS;
                case "promotion_category":
                    return NavigatorCategoryType.PROMOTION_CATEGORY;
                case "my_rights":
                    return NavigatorCategoryType.MY_RIGHTS;
            }
        }
    }
}
