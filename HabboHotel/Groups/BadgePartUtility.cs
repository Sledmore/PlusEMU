namespace Plus.HabboHotel.Groups
{
    public static class BadgePartUtility
    {
        public static string WorkBadgeParts(bool isBase, string partId, string colour, string position)
        {
            string Parts = (isBase) ? "b" : "s";
            partId = (int.Parse(partId) < 10) ? "0" + partId : partId;
            colour = (int.Parse(colour) < 10) ? "0" + colour : colour;
            Parts += partId + colour + position;

            if (!isBase && (Parts == "s00000" || Parts == "s000000"))
            {
                return string.Empty;
            }

            return Parts;
        }
    }
}
