namespace Plus.Core.FigureData.Types
{
    public class Color
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public int ClubLevel { get; set; }
        public bool Selectable { get; set; }
        public string Value { get; set; }

        public Color(int id, int index, int clubLevel, bool selectable, string value)
        {
            Id = id;
            Index = index;
            ClubLevel = clubLevel;
            Selectable = selectable;
            Value = value;
        }
    }
}
