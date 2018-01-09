namespace Plus.HabboHotel.Rooms.PathFinding
{
    public class Vector2D
    {
        public static Vector2D Zero = new Vector2D(0, 0);

        public Vector2D()
        {
        }

        public Vector2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int GetDistanceSquared(Vector2D Point)
        {
            int dx = X - Point.X;
            int dy = Y - Point.Y;
            return (dx * dx) + (dy * dy);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2D)
            {
                var v2d = (Vector2D)obj;
                return v2d.X == X && v2d.Y == Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (X + " " + Y).GetHashCode();
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }

        public static Vector2D operator +(Vector2D One, Vector2D Two)
        {
            return new Vector2D(One.X + Two.X, One.Y + Two.Y);
        }

        public static Vector2D operator -(Vector2D One, Vector2D Two)
        {
            return new Vector2D(One.X - Two.X, One.Y - Two.Y);
        }
    }
}