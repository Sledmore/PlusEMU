namespace Plus.HabboHotel.Rooms.PathFinding
{
    public static class Rotation
    {
        public static int Calculate(int X1, int Y1, int X2, int Y2)
        {
            int Rotation = 0;

            if (X1 > X2 && Y1 > Y2)
                Rotation = 7;

            else if (X1 < X2 && Y1 < Y2)
                Rotation = 3;

            else if (X1 > X2 && Y1 < Y2)
                Rotation = 5;

            else if (X1 < X2 && Y1 > Y2)
                Rotation = 1;

            else if (X1 > X2)
                Rotation = 6;

            else if (X1 < X2)
                Rotation = 2;

            else if (Y1 < Y2)
                Rotation = 4;

            else if (Y1 > Y2)
                Rotation = 0;

            return Rotation;
        }

        public static int Calculate(int X1, int Y1, int X2, int Y2, bool moonwalk)
        {
            int rot = Calculate(X1, Y1, X2, Y2);

            if (!moonwalk)
                return rot;

            return RotationIverse(rot);
        }

        public static int RotationIverse(int rot)
        {
            if (rot > 3)
                rot = rot - 4;
            else
                rot = rot + 4;

            return rot;
        }
    }
}