using System.Collections.Generic;

namespace Plus.HabboHotel.Rooms.PathFinding
{
    public static class PathFinder
    {
        public static Vector2D[] DiagMovePoints = new[]
            {
                new Vector2D(-1, -1),
                new Vector2D(0, -1),
                new Vector2D(1, -1),
                new Vector2D(1, 0),
                new Vector2D(1, 1),
                new Vector2D(0, 1),
                new Vector2D(-1, 1),
                new Vector2D(-1, 0)
            };

        public static Vector2D[] NoDiagMovePoints = new[]
            {
                new Vector2D(0, -1),
                new Vector2D(1, 0),
                new Vector2D(0, 1),
                new Vector2D(-1, 0)
            };

        public static List<Vector2D> FindPath(RoomUser User, bool Diag, Gamemap Map, Vector2D Start, Vector2D End)
        {
            var Path = new List<Vector2D>();

            PathFinderNode Nodes = FindPathReversed(User, Diag, Map, Start, End);

            if (Nodes != null)
            {
                Path.Add(End);

                while (Nodes.Next != null)
                {
                    Path.Add(Nodes.Next.Position);
                    Nodes = Nodes.Next;
                }
            }

            return Path;
        }

        public static PathFinderNode FindPathReversed(RoomUser User, bool Diag, Gamemap Map, Vector2D Start,
                                                      Vector2D End)
        {
            var OpenList = new MinHeap<PathFinderNode>(256);

            var PfMap = new PathFinderNode[Map.Model.MapSizeX, Map.Model.MapSizeY];
            PathFinderNode Node;
            Vector2D Tmp;
            int Cost;
            int Diff;

            var current = new PathFinderNode(Start)
            {
                Cost = 0
            };

            var Finish = new PathFinderNode(End);
            PfMap[current.Position.X, current.Position.Y] = current;
            OpenList.Add(current);

            while (OpenList.Count > 0)
            {
                current = OpenList.ExtractFirst();
                current.InClosed = true;

                for (int i = 0; Diag ? i < DiagMovePoints.Length : i < NoDiagMovePoints.Length; i++)
                {
                    Tmp = current.Position + (Diag ? DiagMovePoints[i] : NoDiagMovePoints[i]);
                    bool IsFinalMove = (Tmp.X == End.X && Tmp.Y == End.Y);

                    if (Map.IsValidStep(new Vector2D(current.Position.X, current.Position.Y), Tmp, IsFinalMove, User.AllowOverride))
                    {
                        if (PfMap[Tmp.X, Tmp.Y] == null)
                        {
                            Node = new PathFinderNode(Tmp);
                            PfMap[Tmp.X, Tmp.Y] = Node;
                        }
                        else
                        {
                            Node = PfMap[Tmp.X, Tmp.Y];
                        }

                        if (!Node.InClosed)
                        {
                            Diff = 0;

                            if (current.Position.X != Node.Position.X)
                            {
                                Diff += 1;
                            }

                            if (current.Position.Y != Node.Position.Y)
                            {
                                Diff += 1;
                            }

                            Cost = current.Cost + Diff + Node.Position.GetDistanceSquared(End);

                            if (Cost < Node.Cost)
                            {
                                Node.Cost = Cost;
                                Node.Next = current;
                            }

                            if (!Node.InOpen)
                            {
                                if (Node.Equals(Finish))
                                {
                                    Node.Next = current;
                                    return Node;
                                }

                                Node.InOpen = true;
                                OpenList.Add(Node);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}