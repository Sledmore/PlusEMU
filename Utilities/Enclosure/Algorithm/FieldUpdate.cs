namespace Plus.Utilities.Enclosure.Algorithm
{
    public class FieldUpdate
    {
        public FieldUpdate(int x, int y, byte value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public byte Value { get; }
        public int Y { get; }
        public int X { get; }
    }
}