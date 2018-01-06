namespace Plus.HabboHotel.Groups
{
    public class GroupBadgeParts
    {
        public int Id { get; }
        public string AssetOne { get; }
        public string AssetTwo { get; }

        public GroupBadgeParts(int id, string assetOne, string assetTwo)
        {
            Id = id;
            AssetOne = assetOne;
            AssetTwo = assetTwo;
        }
    }
}
