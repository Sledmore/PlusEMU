using System.Collections.Generic;
using System.Drawing;

namespace Plus.Utilities.Enclosure
{
    public class PointField
    {
        private static readonly Point badPoint = new Point(-1, -1);
        private readonly List<Point> PointList;
        private Point mostDown = badPoint;
        private Point mostLeft = badPoint;
        private Point mostRight = badPoint;
        private Point mostTop = badPoint;

        public PointField(byte forValue)
        {
            PointList = new List<Point>();
            this.forValue = forValue;
        }

        public byte forValue { get; private set; }

        public List<Point> getPoints()
        {
            return PointList;
        }

        public void add(Point p)
        {
            if (mostLeft == badPoint)
                mostLeft = p;
            if (mostRight == badPoint)
                mostRight = p;
            if (mostTop == badPoint)
                mostTop = p;
            if (mostDown == badPoint)
                mostDown = p;

            if (p.X < mostLeft.X)
                mostLeft = p;
            if (p.X > mostRight.X)
                mostRight = p;
            if (p.Y > mostTop.Y)
                mostTop = p;
            if (p.Y < mostDown.Y)
                mostDown = p;


            PointList.Add(p);
        }
    }
}