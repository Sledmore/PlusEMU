namespace Plus.Core.FigureData.Types
{
    public static class SetTypeUtility
    {
        public static SetType GetSetType(string type)
        {
            switch (type)
            {
                default:
                case "hr":
                    return SetType.HR;
                case "HD":
                    return SetType.HD;
                case "CH":
                    return SetType.CH;
                case "LG":
                    return SetType.LG;
                case "SH":
                    return SetType.SH;
                case "HA":
                    return SetType.HA;
                case "HE":
                    return SetType.HE;
                case "EA":
                    return SetType.EA;
                case "FA":
                    return SetType.FA;
                case "CA":
                    return SetType.CA;
                case "WA":
                    return SetType.WA;
                case "CC":
                    return SetType.CC;
                case "CP":
                    return SetType.CP;
            }
        }
    }
}
