using System;

namespace Plus.HabboHotel.Rooms.PathFinding
{
    public static class Rotation
    {
        public static int Calculate(int X1, int Y1, int X2, int Y2)
        {
            int Rotation = 0;

            int dx = X2 - X1;
            int dy = Y2 - Y1;

            // Calculate the angle between the two points
            double angle = Math.Atan2(dy, dx);

            // Convert to a number 0-7
            double octant = Math.Round(8 * angle / (2 * Math.PI)) + 8;

            // We add 2 to align with existing orientation, as the above calculation is 90deg off.
            Rotation = ((int)octant + 2) % 8;

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