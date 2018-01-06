namespace Plus.HabboHotel.Groups
{
    public static class BadgePartUtility
    {
        public static string WorkBadgeParts(bool isBase, string partId, string colour, string position)
        {
            string parts = isBase ? "b" : "s";
            partId = (int.Parse(partId) < 10) ? "0" + partId : partId;
            colour = (int.Parse(colour) < 10) ? "0" + colour : colour;
            parts += partId + colour + position;

            if (!isBase && (parts == "s00000" || parts == "s000000"))
            {
                return string.Empty;
            }

            return parts;
        }
    }
}
