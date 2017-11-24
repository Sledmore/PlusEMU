using System;
using System.Collections.Generic;
using System.Drawing;
using Astar.Algorithm;
using Plus.Utilities.Enclosure.Algorithm;

namespace Plus.Utilities.Enclosure
{
    public class GameField : IPathNode
    {
        private readonly AStarSolver<GameField> astarSolver;
        private readonly bool diagonal;
        private readonly Queue<GametileUpdate> _newEntries;// = new Queue<GametileUpdate>();
        private byte[,] currentField;
        private GametileUpdate currentlyChecking;

        public GameField(byte[,] theArray, bool diagonalAllowed)
        {
            currentField = theArray;
            diagonal = diagonalAllowed;
            this._newEntries = new Queue<GametileUpdate>();
            astarSolver = new AStarSolver<GameField>(diagonalAllowed, AStarHeuristicType.EXPERIMENTAL_SEARCH, this, theArray.GetUpperBound(1) + 1, theArray.GetUpperBound(0) + 1);
        }

        public bool this[int y, int x]
        {
            get
            {
                if (currentField == null)
                    return false;

                if (y < 0 || x < 0)
                    return false;
                else if (y > currentField.GetUpperBound(0) || x > currentField.GetUpperBound(1))
                    return false;
                return true;
            }
        }

        public bool IsBlocked(int x, int y, bool lastTile)
        {
            if (currentlyChecking.x == x && currentlyChecking.y == y)
                return true;
            return !(getValue(x, y) == currentlyChecking.value);
        }

        public void updateLocation(int x, int y, byte value)
        {
            _newEntries.Enqueue(new GametileUpdate(x, y, value));
        }

        public List<PointField> doUpdate()
        {
            var returnList = new List<PointField>();
            while (_newEntries.Count > 0)
            {
                currentlyChecking = _newEntries.Dequeue();

                List<Point> pointList = getConnectedItems(currentlyChecking);
                if (pointList == null)
                    return null;

                if (pointList.Count > 1)
                {
                    List<LinkedList<AStarSolver<GameField>.PathNode>> RouteList = handleListOfConnectedPoints(  pointList);

                    foreach (var nodeList in RouteList)
                    {
                        if (nodeList.Count >= 4)
                        {
                            PointField field = findClosed(nodeList);
                            if (field != null)
                            {
                                returnList.Add(field);
                            }
                        }
                    }
                }

                currentField[currentlyChecking.y, currentlyChecking.x] = currentlyChecking.value;

            }
            return returnList;
        }

        private PointField findClosed(LinkedList<AStarSolver<GameField>.PathNode> nodeList)
        {
            var returnList = new PointField(currentlyChecking.value);

            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach (AStarSolver<GameField>.PathNode node in nodeList)
            {
                if (node.X < minX)
                    minX = node.X;

                if (node.X > maxX)
                    maxX = node.X;

                if (node.Y < minY)
                    minY = node.Y;

                if (node.Y > maxY)
                    maxY = node.Y;
            }

            int middleX = Convert.ToInt32(Math.Ceiling(((maxX - minX) / 2f)) + minX);
            int middleY = Convert.ToInt32(Math.Ceiling(((maxY - minY) / 2f)) + minY);
            //Console.WriteLine("Middle: x:[{0}]  y:[{1}]", middleX, middleY);

            Point current;
            var toFill = new List<Point>();
            var checkedItems = new List<Point>();
            checkedItems.Add(new Point(currentlyChecking.x, currentlyChecking.y));
            Point toAdd;
            toFill.Add(new Point(middleX, middleY));
            int x;
            int y;
            while (toFill.Count > 0)
            {
                current = toFill[0];
                x = current.X;
                y = current.Y;

                if (x < minX)
                    return null; //OOB
                if (x > maxX)
                    return null; //OOB
                if (y < minY)
                    return null; //OOB
                if (y > maxY)
                    return null; //OOB

                if (this[y - 1, x] && currentField[y - 1, x] == 0)
                {
                    toAdd = new Point(x, y - 1);
                    if (!toFill.Contains(toAdd) && !checkedItems.Contains(toAdd))
                        toFill.Add(toAdd);
                }
                if (this[y + 1, x] && currentField[y + 1, x] == 0)
                {
                    toAdd = new Point(x, y + 1);
                    if (!toFill.Contains(toAdd) && !checkedItems.Contains(toAdd))
                        toFill.Add(toAdd);
                }
                if (this[y, x - 1] && currentField[y, x - 1] == 0)
                {
                    toAdd = new Point(x - 1, y);
                    if (!toFill.Contains(toAdd) && !checkedItems.Contains(toAdd))
                        toFill.Add(toAdd);
                }
                if (this[y, x + 1] && currentField[y, x + 1] == 0)
                {
                    toAdd = new Point(x + 1, y);
                    if (!toFill.Contains(toAdd) && !checkedItems.Contains(toAdd))
                        toFill.Add(toAdd);
                }
                if (getValue(current) == 0)
                    returnList.add(current);
                checkedItems.Add(current);
                toFill.RemoveAt(0);
            }

            return returnList;
        }

        private List<LinkedList<AStarSolver<GameField>.PathNode>> handleListOfConnectedPoints(List<Point> pointList)
        {
            var returnList = new List<LinkedList<AStarSolver<GameField>.PathNode>>();
            int amount = 0;
            foreach (Point begin in pointList)
            {
                amount++;
                if (amount == pointList.Count / 2 + 1)
                    return returnList;
                foreach (Point end in pointList)
                {
                    if (begin == end)
                        continue;
                    LinkedList<AStarSolver<GameField>.PathNode> list = astarSolver.Search(end, begin);
                    if (list != null)
                    {
                        returnList.Add(list);
                    }
                }
            }
            return returnList;
        }

        private List<Point> getConnectedItems(GametileUpdate update)
        {
            if (update == null)
                return null;

            var ConnectedItems = new List<Point>();
            int x = update.x;
            int y = update.y;
            if (diagonal)
            {
                if (this[y - 1, x - 1] && currentField[y - 1, x - 1] == update.value)
                {
                    ConnectedItems.Add(new Point(x - 1, y - 1));
                }
                if (this[y - 1, x + 1] && currentField[y - 1, x + 1] == update.value)
                {
                    ConnectedItems.Add(new Point(x + 1, y - 1));
                }
                if (this[y + 1, x - 1] && currentField[y + 1, x - 1] == update.value)
                {
                    ConnectedItems.Add(new Point(x - 1, y + 1));
                }
                if (this[y + 1, x + 1] && currentField[y + 1, x + 1] == update.value)
                {
                    ConnectedItems.Add(new Point(x + 1, y + 1));
                }
            }


            if (this[y - 1, x] && currentField[y - 1, x] == update.value)
            {
                ConnectedItems.Add(new Point(x, y - 1));
            }
            if (this[y + 1, x] && currentField[y + 1, x] == update.value)
            {
                ConnectedItems.Add(new Point(x, y + 1));
            }
            if (this[y, x - 1] && currentField[y, x - 1] == update.value)
            {
                ConnectedItems.Add(new Point(x - 1, y));
            }
            if (this[y, x + 1] && currentField[y, x + 1] == update.value)
            {
                ConnectedItems.Add(new Point(x + 1, y));
            }

            return ConnectedItems;
        }

        private void setValue(int x, int y, byte value)
        {
            if (this[y, x])
            {
                currentField[y, x] = value;
            }
        }

        public byte getValue(int x, int y)
        {
            if (this[y, x])
            {
                return currentField[y, x];
            }
            return 0;
        }

        public byte getValue(Point p)
        {
            if (this[p.Y, p.X])
            {
                return currentField[p.Y, p.X];
            }
            return 0;
        }

        public void Dispose()
        {
            currentField = null;

            if (this._newEntries != null)
                this._newEntries.Clear();
        }
    }
}