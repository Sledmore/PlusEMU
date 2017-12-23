namespace Plus.HabboHotel.Navigator
{
    public static class NavigatorViewModeUtility
    {
        public static NavigatorViewMode GetViewModeByString(string mode)
        {
            switch (mode.ToUpper())
            {
                default:
                case "REGULAR":
                    return NavigatorViewMode.Regular;
                case "THUMBNAIL":
                    return NavigatorViewMode.Thumbnail;
            }
        }
    }
}
