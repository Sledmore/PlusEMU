using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Navigator
{
    public static class NavigatorSearchAllowanceUtility
    {
        public static NavigatorSearchAllowance GetSearchAllowanceByString(string CategoryType)
        {
            switch (CategoryType.ToUpper())
            {
                default:
                case "NOTHING":
                    return NavigatorSearchAllowance.Nothing;
                case "SHOW_MORE":
                    return NavigatorSearchAllowance.ShowMore;
                case "GO_BACK":
                    return NavigatorSearchAllowance.GoBack;
            }
        }

        public static int GetIntegerValue(NavigatorSearchAllowance SearchAllowance)
        {
            switch (SearchAllowance)
            {
                default:
                case NavigatorSearchAllowance.Nothing:
                    return 0;
                case NavigatorSearchAllowance.ShowMore:
                    return 1;
                case NavigatorSearchAllowance.GoBack:
                    return 2;
            }
        }
    }
}
