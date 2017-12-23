namespace Plus.HabboHotel.Navigator
{
    public static class NavigatorSearchAllowanceUtility
    {
        public static NavigatorSearchAllowance GetSearchAllowanceByString(string type)
        {
            switch (type.ToUpper())
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

        public static int GetIntegerValue(NavigatorSearchAllowance allowance)
        {
            switch (allowance)
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
