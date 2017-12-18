using System;
using System.Text;

namespace Plus.HabboHotel.Rooms
{
    public class DynamicRoomModel
    {
        public bool ClubOnly;
        public int DoorOrientation;
        public int DoorX;
        public int DoorY;
        public double DoorZ;

        public string Heightmap;

        public int MapSizeX;
        public int MapSizeY;
        public short[,] SqFloorHeight;
        public byte[,] SqSeatRot;
        public SquareState[,] SqState;
        private RoomModel staticModel;

        private string RelativeHeightmap = null;

        public DynamicRoomModel(RoomModel pModel)
        {
            staticModel = pModel;
            DoorX = staticModel.DoorX;
            DoorY = staticModel.DoorY;
            DoorZ = staticModel.DoorZ;

            DoorOrientation = staticModel.DoorOrientation;
            Heightmap = staticModel.Heightmap;

            MapSizeX = staticModel.MapSizeX;
            MapSizeY = staticModel.MapSizeY;
            ClubOnly = staticModel.ClubOnly;

            this.RelativeHeightmap = string.Empty;

            Generate();
        }

        public void Generate()
        {
            SqState = new SquareState[MapSizeX, MapSizeY];
            SqFloorHeight = new short[MapSizeX, MapSizeY];
            SqSeatRot = new byte[MapSizeX, MapSizeY];

            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (x > (staticModel.MapSizeX - 1) || y > (staticModel.MapSizeY - 1))
                    {
                        SqState[x, y] = SquareState.Blocked;
                    }
                    else
                    {
                        SqState[x, y] = staticModel.SqState[x, y];
                        SqFloorHeight[x, y] = staticModel.SqFloorHeight[x, y];
                        SqSeatRot[x, y] = staticModel.SqSeatRot[x, y];
                    }
                }
            }

            this.Make();
        }

        private void Make()
        {
            var FloorMap = new StringBuilder();

            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (x == DoorX && y == DoorY)
                    {
                        FloorMap.Append(DoorZ > 9 ? ((char)(87 + DoorZ)).ToString() : DoorZ.ToString());
                        continue;
                    }

                    if (SqState[x, y] == SquareState.Blocked)
                    {
                        FloorMap.Append('x');
                        continue;
                    }

                    double Height = SqFloorHeight[x, y];
                    string Val = Height > 9 ? ((char)(87 + Height)).ToString() : Height.ToString();
                    FloorMap.Append(Val);

                }
                FloorMap.Append(Convert.ToChar(13));
            }

            this.RelativeHeightmap = FloorMap.ToString();
        }

        public void refreshArrays()
        {
            var newSqState = new SquareState[MapSizeX + 1, MapSizeY + 1];
            var newSqFloorHeight = new short[MapSizeX + 1, MapSizeY + 1];
            var newSqSeatRot = new byte[MapSizeX + 1, MapSizeY + 1];

            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (x > (staticModel.MapSizeX - 1) || y > (staticModel.MapSizeY - 1))
                    {
                        newSqState[x, y] = SquareState.Blocked;
                    }
                    else
                    {
                        newSqState[x, y] = SqState[x, y];
                        newSqFloorHeight[x, y] = SqFloorHeight[x, y];
                        newSqSeatRot[x, y] = SqSeatRot[x, y];
                    }
                }
            }

            SqState = newSqState;
            SqFloorHeight = newSqFloorHeight;
            SqSeatRot = newSqSeatRot;

        }

        public string GetRelativeHeightmap()
        {
            return this.RelativeHeightmap;
        }



        public void AddX()
        {
            MapSizeX++;
            refreshArrays();
        }

        public void OpenSquare(int x, int y, double z)
        {
            if (z > 9)
                z = 9;
            if (z < 0)
                z = 0;
            SqFloorHeight[x, y] = (short)z;
            SqState[x, y] = SquareState.Open;
        }

        public void AddY()
        {
            MapSizeY++;
            refreshArrays();
        }

        public bool DoorIsValid()
        {
            if (DoorX > SqFloorHeight.GetUpperBound(0) || DoorY > SqFloorHeight.GetUpperBound(1))
                return false;
            else
                return true;
        }

        public void SetMapsize(int x, int y)
        {
            MapSizeX = x;
            MapSizeY = y;
            refreshArrays();
        }

        public void Destroy()
        {
            Array.Clear(SqState, 0, SqState.Length);
            Array.Clear(SqFloorHeight, 0, SqFloorHeight.Length);
            Array.Clear(SqSeatRot, 0, SqSeatRot.Length);

            staticModel = null;
            Heightmap = null;
            SqState = null;
            SqFloorHeight = null;
            SqSeatRot = null;
        }
    }
}