namespace Plus.Core.FigureData.Types
{
    class Part
    {
        public int Id { get; set; }
        public SetType SetType { get; set; }
        public bool Colorable { get; set; }
        public int Index { get; set; }
        public int ColorIndex { get; set; }


        public Part(int id, SetType setType, bool colorable, int index, int colorIndex)
        {
            Id = id;
            SetType = setType;
            Colorable = colorable;
            Index = index;
            ColorIndex = colorIndex;
        }
    }
}
