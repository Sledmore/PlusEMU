using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Plus.Core;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Rooms.Games.Teams;
using System.Collections.Concurrent;
using Plus.HabboHotel.Rooms.PathFinding;

namespace Plus.HabboHotel.Rooms
{
    public class Gamemap
    {
        private Room _room;
        private byte[,] _gameMap; // 0 = none, 1 = pool, 2 = normal skates, 3 = ice skates

        public bool DiagonalEnabled { get; set; }
        private RoomModel _model;
        private byte[,] _userItemEffect;
        private double[,] _itemHeightMap;
        private DynamicRoomModel _dynamicModel;
        private ConcurrentDictionary<Point, List<int>> _coordinatedItems;
        private ConcurrentDictionary<Point, List<RoomUser>> _userMap;

        public Gamemap(Room room, RoomModel model)
        {
            _room = room;
            _model = model;
            DiagonalEnabled = true;

            _dynamicModel = new DynamicRoomModel(this._model);
            _coordinatedItems = new ConcurrentDictionary<Point, List<int>>();
            _itemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];
            _userMap = new ConcurrentDictionary<Point, List<RoomUser>>();
        }

        public void AddUserToMap(RoomUser user, Point coord)
        {
            if (_userMap.ContainsKey(coord))
            {
                _userMap[coord].Add(user);
            }
            else
            {
                List<RoomUser> users = new List<RoomUser>();
                users.Add(user);
                _userMap.TryAdd(coord, users);
            }
        }

        public void TeleportToItem(RoomUser user, Item item)
        {
            if (item == null || user == null)
                return;

            GameMap[user.X, user.Y] = user.SqState;
            UpdateUserMovement(new Point(user.Coordinate.X, user.Coordinate.Y), new Point(item.Coordinate.X, item.Coordinate.Y), user);
            user.X = item.GetX;
            user.Y = item.GetY;
            user.Z = item.GetZ;

            user.SqState = GameMap[item.GetX, item.GetY];
            GameMap[user.X, user.Y] = 1;
            user.RotBody = item.Rotation;
            user.RotHead = item.Rotation;

            user.GoalX = user.X;
            user.GoalY = user.Y;
            user.SetStep = false;
            user.IsWalking = false;
            user.UpdateNeeded = true;
        }

        public void UpdateUserMovement(Point oldCoord, Point newCoord, RoomUser user)
        {
            RemoveUserFromMap(user, oldCoord);
            AddUserToMap(user, newCoord);
        }

        public void RemoveUserFromMap(RoomUser user, Point coord)
        {
            if (_userMap.ContainsKey(coord))
                ((List<RoomUser>)_userMap[coord]).RemoveAll(x => x != null && x.VirtualId == user.VirtualId);
        }

        public bool MapGotUser(Point coord)
        {
            return (GetRoomUsers(coord).Count > 0);
        }

        public List<RoomUser> GetRoomUsers(Point coord)
        {
            if (_userMap.ContainsKey(coord))
                return (List<RoomUser>)_userMap[coord];
            else
                return new List<RoomUser>();
        }

        public Point getRandomWalkableSquare()
        {
            var walkableSquares = new List<Point>();
            for (int y = 0; y < _gameMap.GetUpperBound(1); y++)
            {
                for (int x = 0; x < _gameMap.GetUpperBound(0); x++)
                {
                    if (_model.DoorX != x && _model.DoorY != y && _gameMap[x, y] == 1)
                        walkableSquares.Add(new Point(x, y));
                }
            }

            int RandomNumber = PlusEnvironment.GetRandomNumber(0, walkableSquares.Count);
            int i = 0;

            foreach (Point coord in walkableSquares.ToList())
            {
                if (i == RandomNumber)
                    return coord;
                i++;
            }

            return new Point(0, 0);
        }


        public bool isInMap(int X, int Y)
        {
            var walkableSquares = new List<Point>();
            for (int y = 0; y < _gameMap.GetUpperBound(1); y++)
            {
                for (int x = 0; x < _gameMap.GetUpperBound(0); x++)
                {
                    if (_model.DoorX != x && _model.DoorY != y && _gameMap[x, y] == 1)
                        walkableSquares.Add(new Point(x, y));
                }
            }

            if (walkableSquares.Contains(new Point(X, Y)))
                return true;
            return false;
        }

        public void AddToMap(Item item)
        {
            AddItemToMap(item);
        }

        private void SetDefaultValue(int x, int y)
        {
            _gameMap[x, y] = 0;
            _userItemEffect[x, y] = 0;
            _itemHeightMap[x, y] = 0.0;

            if (x == Model.DoorX && y == Model.DoorY)
            {
                _gameMap[x, y] = 3;
            }
            else if (Model.SqState[x, y] == SquareState.Open)
            {
                _gameMap[x, y] = 1;
            }
            else if (Model.SqState[x, y] == SquareState.Seat)
            {
                _gameMap[x, y] = 2;
            }
        }

        public void updateMapForItem(Item item)
        {
            RemoveFromMap(item);
            AddToMap(item);
        }

        public void GenerateMaps(bool checkLines = true)
        {
            int MaxX = 0;
            int MaxY = 0;
            _coordinatedItems = new ConcurrentDictionary<Point, List<int>>();

            if (checkLines)
            {
                Item[] items = _room.GetRoomItemHandler().GetFloor.ToArray();
                foreach (Item item in items.ToList())
                {
                    if (item == null)
                        continue;

                    if (item.GetX > Model.MapSizeX && item.GetX > MaxX)
                        MaxX = item.GetX;
                    if (item.GetY > Model.MapSizeY && item.GetY > MaxY)
                        MaxY = item.GetY;
                }

                Array.Clear(items, 0, items.Length);
                items = null;
            }


            if (MaxY > (Model.MapSizeY - 1) || MaxX > (Model.MapSizeX - 1))
            {
                if (MaxX < Model.MapSizeX)
                    MaxX = Model.MapSizeX;
                if (MaxY < Model.MapSizeY)
                    MaxY = Model.MapSizeY;

                Model.SetMapsize(MaxX + 7, MaxY + 7);
                GenerateMaps(false);
                return;
            }

            if (MaxX != StaticModel.MapSizeX || MaxY != StaticModel.MapSizeY)
            {
                _userItemEffect = new byte[Model.MapSizeX, Model.MapSizeY];
                _gameMap = new byte[Model.MapSizeX, Model.MapSizeY];


                _itemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];
                //if (modelRemap)
                //    Model.Generate(); //Clears model

                for (int line = 0; line < Model.MapSizeY; line++)
                {
                    for (int chr = 0; chr < Model.MapSizeX; chr++)
                    {
                        _gameMap[chr, line] = 0;
                        _userItemEffect[chr, line] = 0;

                        if (chr == Model.DoorX && line == Model.DoorY)
                        {
                            _gameMap[chr, line] = 3;
                        }
                        else if (Model.SqState[chr, line] == SquareState.Open)
                        {
                            _gameMap[chr, line] = 1;
                        }
                        else if (Model.SqState[chr, line] == SquareState.Seat)
                        {
                            _gameMap[chr, line] = 2;
                        }
                        else if (Model.SqState[chr, line] == SquareState.Pool)
                        {
                            _userItemEffect[chr, line] = 6;
                        }
                    }
                }
            }
            else
            {
                _userItemEffect = new byte[Model.MapSizeX, Model.MapSizeY];
                _gameMap = new byte[Model.MapSizeX, Model.MapSizeY];


                _itemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];

                for (int line = 0; line < Model.MapSizeY; line++)
                {
                    for (int chr = 0; chr < Model.MapSizeX; chr++)
                    {
                        _gameMap[chr, line] = 0;
                        _userItemEffect[chr, line] = 0;

                        if (chr == Model.DoorX && line == Model.DoorY)
                        {
                            _gameMap[chr, line] = 3;
                        }
                        else if (Model.SqState[chr, line] == SquareState.Open)
                        {
                            _gameMap[chr, line] = 1;
                        }
                        else if (Model.SqState[chr, line] == SquareState.Seat)
                        {
                            _gameMap[chr, line] = 2;
                        }
                        else if (Model.SqState[chr, line] == SquareState.Pool)
                        {
                            _userItemEffect[chr, line] = 6;
                        }
                    }
                }
            }

            Item[] tmpItems = _room.GetRoomItemHandler().GetFloor.ToArray();
            foreach (Item Item in tmpItems.ToList())
            {
                if (Item == null)
                    continue;

                if (!AddItemToMap(Item))
                    continue;
            }
            Array.Clear(tmpItems, 0, tmpItems.Length);
            tmpItems = null;

            if (_room.RoomBlockingEnabled == 0)
            {
                foreach (RoomUser user in _room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (user == null)
                        continue;

                    user.SqState = _gameMap[user.X, user.Y];
                    _gameMap[user.X, user.Y] = 0;
                }
            }

            try
            {
                _gameMap[Model.DoorX, Model.DoorY] = 3;
            }
            catch { }
        }

        private bool ConstructMapForItem(Item Item, Point Coord)
        {
            try
            {
                if (Coord.X > (Model.MapSizeX - 1))
                {
                    Model.AddX();
                    GenerateMaps();
                    return false;
                }

                if (Coord.Y > (Model.MapSizeY - 1))
                {
                    Model.AddY();
                    GenerateMaps();
                    return false;
                }

                if (Model.SqState[Coord.X, Coord.Y] == SquareState.Blocked)
                {
                    Model.OpenSquare(Coord.X, Coord.Y, Item.GetZ);
                }
                if (_itemHeightMap[Coord.X, Coord.Y] <= Item.TotalHeight)
                {
                    _itemHeightMap[Coord.X, Coord.Y] = Item.TotalHeight - _dynamicModel.SqFloorHeight[Item.GetX, Item.GetY];
                    _userItemEffect[Coord.X, Coord.Y] = 0;


                    switch (Item.GetBaseItem().InteractionType)
                    {
                        case InteractionType.POOL:
                            _userItemEffect[Coord.X, Coord.Y] = 1;
                            break;
                        case InteractionType.NORMAL_SKATES:
                            _userItemEffect[Coord.X, Coord.Y] = 2;
                            break;
                        case InteractionType.ICE_SKATES:
                            _userItemEffect[Coord.X, Coord.Y] = 3;
                            break;
                        case InteractionType.lowpool:
                            _userItemEffect[Coord.X, Coord.Y] = 4;
                            break;
                        case InteractionType.haloweenpool:
                            _userItemEffect[Coord.X, Coord.Y] = 5;
                            break;
                    }


                    //SwimHalloween
                    if (Item.GetBaseItem().Walkable)    // If this item is walkable and on the floor, allow users to walk here.
                    {
                        if (_gameMap[Coord.X, Coord.Y] != 3)
                            _gameMap[Coord.X, Coord.Y] = 1;
                    }
                    else if (Item.GetZ <= (Model.SqFloorHeight[Item.GetX, Item.GetY] + 0.1) && Item.GetBaseItem().InteractionType == InteractionType.GATE && Item.ExtraData == "1")// If this item is a gate, open, and on the floor, allow users to walk here.
                    {
                        if (_gameMap[Coord.X, Coord.Y] != 3)
                            _gameMap[Coord.X, Coord.Y] = 1;
                    }
                    else if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.BED || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                    {
                        _gameMap[Coord.X, Coord.Y] = 3;
                    }
                    else // Finally, if it's none of those, block the square.
                    {
                        if (_gameMap[Coord.X, Coord.Y] != 3)
                            _gameMap[Coord.X, Coord.Y] = 0;
                    }
                }

                // Set bad maps
                if (Item.GetBaseItem().InteractionType == InteractionType.BED || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                    _gameMap[Coord.X, Coord.Y] = 3;
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
            return true;
        }

        public void AddCoordinatedItem(Item item, Point coord)
        {
            List<int> Items = new List<int>(); //mCoordinatedItems[CoordForItem];

            if (!_coordinatedItems.TryGetValue(coord, out Items))
            {
                Items = new List<int>();

                if (!Items.Contains(item.Id))
                    Items.Add(item.Id);

                if (!_coordinatedItems.ContainsKey(coord))
                    _coordinatedItems.TryAdd(coord, Items);
            }
            else
            {
                if (!Items.Contains(item.Id))
                {
                    Items.Add(item.Id);
                    _coordinatedItems[coord] = Items;
                }
            }
        }

        public List<Item> GetCoordinatedItems(Point coord)
        {
            var point = new Point(coord.X, coord.Y);
            List<Item> Items = new List<Item>();

            if (_coordinatedItems.ContainsKey(point))
            {
                List<int> Ids = _coordinatedItems[point];
                Items = GetItemsFromIds(Ids);
                return Items;
            }

            return new List<Item>();
        }

        public bool RemoveCoordinatedItem(Item item, Point coord)
        {
            Point point = new Point(coord.X, coord.Y);
            if (_coordinatedItems != null && _coordinatedItems.ContainsKey(point))
            {
                ((List<int>)_coordinatedItems[point]).RemoveAll(x => x == item.Id);
                return true;
            }
            return false;
        }

        private void AddSpecialItems(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GATE:
                    //IsTrans = true;
                    _room.GetSoccer().RegisterGate(item);


                    string[] splittedExtraData = item.ExtraData.Split(':');

                    if (string.IsNullOrEmpty(item.ExtraData) || splittedExtraData.Length <= 1)
                    {
                        item.Gender = "M";
                        switch (item.team)
                        {
                            case Team.Yellow:
                                item.Figure = "lg-275-93.hr-115-61.hd-207-14.ch-265-93.sh-305-62";
                                break;
                            case Team.Red:
                                item.Figure = "lg-275-96.hr-115-61.hd-180-3.ch-265-96.sh-305-62";
                                break;
                            case Team.Green:
                                item.Figure = "lg-275-102.hr-115-61.hd-180-3.ch-265-102.sh-305-62";
                                break;
                            case Team.Blue:
                                item.Figure = "lg-275-108.hr-115-61.hd-180-3.ch-265-108.sh-305-62";
                                break;
                        }
                    }
                    else
                    {
                        item.Gender = splittedExtraData[0];
                        item.Figure = splittedExtraData[1];
                    }
                    break;

                case InteractionType.banzaifloor:
                    {
                        _room.GetBanzai().AddTile(item, item.Id);
                        break;
                    }

                case InteractionType.banzaipyramid:
                    {
                        _room.GetGameItemHandler().AddPyramid(item, item.Id);
                        break;
                    }

                case InteractionType.banzaitele:
                    {
                        _room.GetGameItemHandler().AddTeleport(item, item.Id);
                        item.ExtraData = "";
                        break;
                    }
                case InteractionType.banzaipuck:
                    {
                        _room.GetBanzai().AddPuck(item);
                        break;
                    }

                case InteractionType.FOOTBALL:
                    {
                        _room.GetSoccer().AddBall(item);
                        break;
                    }
                case InteractionType.FREEZE_TILE_BLOCK:
                    {
                        _room.GetFreeze().AddFreezeBlock(item);
                        break;
                    }
                case InteractionType.FREEZE_TILE:
                    {
                        _room.GetFreeze().AddFreezeTile(item);
                        break;
                    }
                case InteractionType.freezeexit:
                    {
                        _room.GetFreeze().AddExitTile(item);
                        break;
                    }
            }
        }

        private void RemoveSpecialItem(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GATE:
                    _room.GetSoccer().UnRegisterGate(item);
                    break;
                case InteractionType.banzaifloor:
                    _room.GetBanzai().RemoveTile(item.Id);
                    break;
                case InteractionType.banzaipuck:
                    _room.GetBanzai().RemovePuck(item.Id);
                    break;
                case InteractionType.banzaipyramid:
                    _room.GetGameItemHandler().RemovePyramid(item.Id);
                    break;
                case InteractionType.banzaitele:
                    _room.GetGameItemHandler().RemoveTeleport(item.Id);
                    break;
                case InteractionType.FOOTBALL:
                    _room.GetSoccer().RemoveBall(item.Id);
                    break;
                case InteractionType.FREEZE_TILE:
                    _room.GetFreeze().RemoveFreezeTile(item.Id);
                    break;
                case InteractionType.FREEZE_TILE_BLOCK:
                    _room.GetFreeze().RemoveFreezeBlock(item.Id);
                    break;
                case InteractionType.freezeexit:
                    _room.GetFreeze().RemoveExitTile(item.Id);
                    break;
            }
        }

        public bool RemoveFromMap(Item item, bool handleGameItem)
        {
            if (handleGameItem)
                RemoveSpecialItem(item);

            if (_room.GotSoccer())
                _room.GetSoccer().OnGateRemove(item);

            bool isRemoved = false;
            foreach (Point coord in item.GetCoords.ToList())
            {
                if (RemoveCoordinatedItem(item, coord))
                    isRemoved = true;
            }

            ConcurrentDictionary<Point, List<Item>> items = new ConcurrentDictionary<Point, List<Item>>();
            foreach (Point Tile in item.GetCoords.ToList())
            {
                Point point = new Point(Tile.X, Tile.Y);
                if (_coordinatedItems.ContainsKey(point))
                {
                    List<int> Ids = (List<int>)_coordinatedItems[point];
                    List<Item> __items = GetItemsFromIds(Ids);

                    if (!items.ContainsKey(Tile))
                        items.TryAdd(Tile, __items);
                }

                SetDefaultValue(Tile.X, Tile.Y);
            }

            foreach (Point Coord in items.Keys.ToList())
            {
                if (!items.ContainsKey(Coord))
                    continue;

                List<Item> SubItems = (List<Item>)items[Coord];
                foreach (Item Item in SubItems.ToList())
                {
                    ConstructMapForItem(Item, Coord);
                }
            }


            items.Clear();
            items = null;


            return isRemoved;
        }

        public bool RemoveFromMap(Item item)
        {
            return RemoveFromMap(item, true);
        }

        public bool AddItemToMap(Item Item, bool handleGameItem, bool NewItem = true)
        {

            if (handleGameItem)
            {
                AddSpecialItems(Item);

                switch (Item.GetBaseItem().InteractionType)
                {
                    case InteractionType.FOOTBALL_GOAL_RED:
                    case InteractionType.footballcounterred:
                    case InteractionType.banzaiscorered:
                    case InteractionType.banzaigatered:
                    case InteractionType.freezeredcounter:
                    case InteractionType.FREEZE_RED_GATE:
                        {
                            if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
                                _room.GetGameManager().AddFurnitureToTeam(Item, Team.Red);
                            break;
                        }
                    case InteractionType.FOOTBALL_GOAL_GREEN:
                    case InteractionType.footballcountergreen:
                    case InteractionType.banzaiscoregreen:
                    case InteractionType.banzaigategreen:
                    case InteractionType.freezegreencounter:
                    case InteractionType.FREEZE_GREEN_GATE:
                        {
                            if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
                                _room.GetGameManager().AddFurnitureToTeam(Item, Team.Green);
                            break;
                        }
                    case InteractionType.FOOTBALL_GOAL_BLUE:
                    case InteractionType.footballcounterblue:
                    case InteractionType.banzaiscoreblue:
                    case InteractionType.banzaigateblue:
                    case InteractionType.freezebluecounter:
                    case InteractionType.FREEZE_BLUE_GATE:
                        {
                            if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
                                _room.GetGameManager().AddFurnitureToTeam(Item, Team.Blue);
                            break;
                        }
                    case InteractionType.FOOTBALL_GOAL_YELLOW:
                    case InteractionType.footballcounteryellow:
                    case InteractionType.banzaiscoreyellow:
                    case InteractionType.banzaigateyellow:
                    case InteractionType.freezeyellowcounter:
                    case InteractionType.FREEZE_YELLOW_GATE:
                        {
                            if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
                                _room.GetGameManager().AddFurnitureToTeam(Item, Team.Yellow);
                            break;
                        }
                    case InteractionType.freezeexit:
                        {
                            _room.GetFreeze().AddExitTile(Item);
                            break;
                        }
                    case InteractionType.ROLLER:
                        {
                            if (!_room.GetRoomItemHandler().GetRollers().Contains(Item))
                                _room.GetRoomItemHandler().TryAddRoller(Item.Id, Item);
                            break;
                        }
                }
            }

            if (Item.GetBaseItem().Type != 's')
                return true;

            foreach (Point coord in Item.GetCoords.ToList())
            {
                AddCoordinatedItem(Item, new Point(coord.X, coord.Y));
            }

            if (Item.GetX > (Model.MapSizeX - 1))
            {
                Model.AddX();
                GenerateMaps();
                return false;
            }

            if (Item.GetY > (Model.MapSizeY - 1))
            {
                Model.AddY();
                GenerateMaps();
                return false;
            }

            bool Return = true;

            foreach (Point coord in Item.GetCoords)
            {
                if (!ConstructMapForItem(Item, coord))
                {
                    Return = false;
                }
                else
                {
                    Return = true;
                }
            }



            return Return;
        }


        public bool CanWalk(int X, int Y, bool Override)
        {

            if (Override)
            {
                return true;
            }

            if (_room.GetRoomUserManager().GetUserForSquare(X, Y) != null && _room.RoomBlockingEnabled == 0)
                return false;

            return true;
        }

        public bool AddItemToMap(Item Item, bool NewItem = true)
        {
            return AddItemToMap(Item, true, NewItem);
        }

        public bool ItemCanMove(Item Item, Point MoveTo)
        {
            List<ThreeDCoord> Points = Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, MoveTo.X, MoveTo.Y, Item.Rotation).Values.ToList();

            if (Points == null || Points.Count == 0)
                return true;

            foreach (ThreeDCoord Coord in Points)
            {

                if (Coord.X >= Model.MapSizeX || Coord.Y >= Model.MapSizeY)
                    return false;

                if (!SquareIsOpen(Coord.X, Coord.Y, false))
                    return false;

                continue;
            }

            return true;
        }

        public byte GetFloorStatus(Point coord)
        {
            if (coord.X > _gameMap.GetUpperBound(0) || coord.Y > _gameMap.GetUpperBound(1))
                return 1;

            return _gameMap[coord.X, coord.Y];
        }

        public void SetFloorStatus(int X, int Y, byte Status)
        {
            _gameMap[X, Y] = Status;
        }

        public double GetHeightForSquareFromData(Point coord)
        {
            if (coord.X > _dynamicModel.SqFloorHeight.GetUpperBound(0) ||
                coord.Y > _dynamicModel.SqFloorHeight.GetUpperBound(1))
                return 1;
            return _dynamicModel.SqFloorHeight[coord.X, coord.Y];
        }

        public bool CanRollItemHere(int x, int y)
        {
            if (!ValidTile(x, y))
                return false;

            if (Model.SqState[x, y] == SquareState.Blocked)
                return false;

            return true;
        }

        public bool SquareIsOpen(int x, int y, bool pOverride)
        {
            if ((_dynamicModel.MapSizeX - 1) < x || (_dynamicModel.MapSizeY - 1) < y)
                return false;

            return CanWalk(_gameMap[x, y], pOverride);
        }

        public bool GetHighestItemForSquare(Point Square, out Item Item)
        {
            List<Item> Items = GetAllRoomItemForSquare(Square.X, Square.Y);
            Item = null;
            double HighestZ = -1;

            if (Items != null && Items.Count() > 0)
            {
                foreach (Item uItem in Items.ToList())
                {
                    if (uItem == null)
                        continue;

                    if (uItem.TotalHeight > HighestZ)
                    {
                        HighestZ = uItem.TotalHeight;
                        Item = uItem;
                        continue;
                    }
                    else
                        continue;
                }
            }
            else
                return false;

            return true;
        }

        public double GetHeightForSquare(Point Coord)
        {
            Item rItem;

            if (GetHighestItemForSquare(Coord, out rItem))
                if (rItem != null)
                    return rItem.TotalHeight;

            return 0.0;
        }

        public Point GetChaseMovement(Item Item)
        {
            int Distance = 99;
            Point Coord = new Point(0, 0);
            int iX = Item.GetX;
            int iY = Item.GetY;
            bool X = false;

            foreach (RoomUser User in _room.GetRoomUserManager().GetRoomUsers())
            {
                if (User.X == Item.GetX || Item.GetY == User.Y)
                {
                    if (User.X == Item.GetX)
                    {
                        int Difference = Math.Abs(User.Y - Item.GetY);
                        if (Difference < Distance)
                        {
                            Distance = Difference;
                            Coord = User.Coordinate;
                            X = false;
                        }
                        else
                            continue;

                    }
                    else if (User.Y == Item.GetY)
                    {
                        int Difference = Math.Abs(User.X - Item.GetX);
                        if (Difference < Distance)
                        {
                            Distance = Difference;
                            Coord = User.Coordinate;
                            X = true;
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }
            }

            if (Distance > 5)
                return Item.GetSides().OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (X && Distance < 99)
            {
                if (iX > Coord.X)
                {
                    iX--;
                    return new Point(iX, iY);
                }
                else
                {
                    iX++;
                    return new Point(iX, iY);
                }
            }
            else if (!X && Distance < 99)
            {
                if (iY > Coord.Y)
                {
                    iY--;
                    return new Point(iX, iY);
                }
                else
                {
                    iY++;
                    return new Point(iX, iY);
                }
            }
            else
                return Item.Coordinate;
        }

        public bool IsValidStep2(RoomUser User, Vector2D From, Vector2D To, bool EndOfPath, bool Override)
        {
            if (User == null)
                return false;

            if (!ValidTile(To.X, To.Y))
                return false;

            if (Override)
                return true;

            /*
             * 0 = blocked
             * 1 = open
             * 2 = last step
             * 3 = door
             * */

            List<Item> Items = _room.GetGameMap().GetAllRoomItemForSquare(To.X, To.Y);
            if (Items.Count > 0)
            {
                bool HasGroupGate = Items.ToList().Count(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE) > 0;
                if (HasGroupGate)
                {
                    Item I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE);
                    if (I != null)
                    {
                        if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(I.GroupId, out Group Group))
                            return false;

                        if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                            return false;

                        if (Group.IsMember(User.GetClient().GetHabbo().Id))
                        {
                            I.InteractingUser = User.GetClient().GetHabbo().Id;
                            I.ExtraData = "1";
                            I.UpdateState(false, true);

                            I.RequestUpdate(4, true);

                            return true;
                        }
                        else
                        {
                            if (User.Path.Count > 0)
                                User.Path.Clear();
                            User.PathRecalcNeeded = false;
                            return false;
                        }
                    }
                }
            }

            bool Chair = false;
            double HighestZ = -1;
            foreach (Item Item in Items.ToList())
            {
                if (Item == null)
                    continue;

                if (Item.GetZ < HighestZ)
                {
                    Chair = false;
                    continue;
                }

                HighestZ = Item.GetZ;
                if (Item.GetBaseItem().IsSeat)
                    Chair = true;
            }

            if ((_gameMap[To.X, To.Y] == 3 && !EndOfPath && !Chair) || (_gameMap[To.X, To.Y] == 0) || (_gameMap[To.X, To.Y] == 2 && !EndOfPath))
            {
                if (User.Path.Count > 0)
                    User.Path.Clear();
                User.PathRecalcNeeded = true;
            }

            double HeightDiff = SqAbsoluteHeight(To.X, To.Y) - SqAbsoluteHeight(From.X, From.Y);
            if (HeightDiff > 1.5 && !User.RidingHorse)
                return false;

            //Check this last, because ya.
            RoomUser Userx = _room.GetRoomUserManager().GetUserForSquare(To.X, To.Y);
            if (Userx != null)
            {
                if (!Userx.IsWalking && EndOfPath)
                    return false;
            }
            return true;
        }
    
        public bool IsValidStep(Vector2D from, Vector2D to, bool endOfPath, bool overriding, bool roller = false)
        {
            if (!ValidTile(to.X, to.Y))
                return false;

            if (overriding)
                return true;

            /*
             * 0 = blocked
             * 1 = open
             * 2 = last step
             * 3 = door
             * */

            if (_room.RoomBlockingEnabled == 0 && SquareHasUsers(to.X, to.Y))
                return false;

            List<Item> items = _room.GetGameMap().GetAllRoomItemForSquare(to.X, to.Y);
            if (items.Count > 0)
            {
                bool HasGroupGate = items.ToList().Count(x => x != null && x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE) > 0;
                if (HasGroupGate)
                    return true;
            }

            if ((_gameMap[to.X, to.Y] == 3 && !endOfPath) || _gameMap[to.X, to.Y] == 0 || (_gameMap[to.X, to.Y] == 2 && !endOfPath))
                return false;

            if (!roller)
            {
                double HeightDiff = SqAbsoluteHeight(to.X, to.Y) - SqAbsoluteHeight(from.X, from.Y);
                if (HeightDiff > 1.5)
                    return false;
            }

            return true;
        }

        public static bool CanWalk(byte state, bool overriding)
        {
            if (!overriding)
            {
                if (state == 3)
                    return true;
                if (state == 1)
                    return true;

                return false;
            }
            return true;
        }

        public bool itemCanBePlacedHere(int x, int y)
        {
            if (_dynamicModel.MapSizeX - 1 < x || _dynamicModel.MapSizeY - 1 < y ||
                (x == _dynamicModel.DoorX && y == _dynamicModel.DoorY))
                return false;

            return _gameMap[x, y] == 1;
        }

        public double SqAbsoluteHeight(int x, int y)
        {
            Point Points = new Point(x, y);


            if (_coordinatedItems.TryGetValue(Points, out List<int> Ids))
            {
                List<Item> Items = GetItemsFromIds(Ids);

                return SqAbsoluteHeight(x, y, Items);
            }
            else
                return _dynamicModel.SqFloorHeight[x, y];

            #region Old
            /*
            if (mCoordinatedItems.ContainsKey(Points))
            {
                List<Item> Items = new List<Item>();
                foreach (Item Item in mCoordinatedItems[Points].ToArray())
                {
                    if (!Items.Contains(Item))
                        Items.Add(Item);
                }
                return SqAbsoluteHeight(X, Y, Items);
            }*/
            #endregion
        }

        public double SqAbsoluteHeight(int X, int Y, List<Item> ItemsOnSquare)
        {
            try
            {
                bool deduct = false;
                double HighestStack = 0;
                double deductable = 0.0;

                if (ItemsOnSquare != null && ItemsOnSquare.Count > 0)
                {
                    foreach (Item Item in ItemsOnSquare.ToList())
                    {
                        if (Item == null)
                            continue;

                        if (Item.TotalHeight > HighestStack)
                        {
                            if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.BED || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                            {
                                deduct = true;
                                deductable = Item.GetBaseItem().Height;
                            }
                            else
                                deduct = false;
                            HighestStack = Item.TotalHeight;
                        }
                    }
                }

                double floorHeight = Model.SqFloorHeight[X, Y];
                double stackHeight = HighestStack - Model.SqFloorHeight[X, Y];

                if (deduct)
                    stackHeight -= deductable;

                if (stackHeight < 0)
                    stackHeight = 0;

                return (floorHeight + stackHeight);
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
                return 0;
            }
        }

        public bool ValidTile(int X, int Y)
        {
            if (X < 0 || Y < 0 || X >= Model.MapSizeX || Y >= Model.MapSizeY)
            {
                return false;
            }

            return true;
        }

        public static Dictionary<int, ThreeDCoord> GetAffectedTiles(int Length, int Width, int PosX, int PosY, int Rotation)
        {
            int x = 0;

            var PointList = new Dictionary<int, ThreeDCoord>();

            if (Length > 1)
            {
                if (Rotation == 0 || Rotation == 4)
                {
                    for (int i = 1; i < Length; i++)
                    {
                        PointList.Add(x++, new ThreeDCoord(PosX, PosY + i, i));

                        for (int j = 1; j < Width; j++)
                        {
                            PointList.Add(x++, new ThreeDCoord(PosX + j, PosY + i, (i < j) ? j : i));
                        }
                    }
                }
                else if (Rotation == 2 || Rotation == 6)
                {
                    for (int i = 1; i < Length; i++)
                    {
                        PointList.Add(x++, new ThreeDCoord(PosX + i, PosY, i));

                        for (int j = 1; j < Width; j++)
                        {
                            PointList.Add(x++, new ThreeDCoord(PosX + i, PosY + j, (i < j) ? j : i));
                        }
                    }
                }
            }

            if (Width > 1)
            {
                if (Rotation == 0 || Rotation == 4)
                {
                    for (int i = 1; i < Width; i++)
                    {
                        PointList.Add(x++, new ThreeDCoord(PosX + i, PosY, i));

                        for (int j = 1; j < Length; j++)
                        {
                            PointList.Add(x++, new ThreeDCoord(PosX + i, PosY + j, (i < j) ? j : i));
                        }
                    }
                }
                else if (Rotation == 2 || Rotation == 6)
                {
                    for (int i = 1; i < Width; i++)
                    {
                        PointList.Add(x++, new ThreeDCoord(PosX, PosY + i, i));

                        for (int j = 1; j < Length; j++)
                        {
                            PointList.Add(x++, new ThreeDCoord(PosX + j, PosY + i, (i < j) ? j : i));
                        }
                    }
                }
            }

            return PointList;
        }

        public List<Item> GetItemsFromIds(List<int> Input)
        {
            if (Input == null || Input.Count == 0)
                return new List<Item>();

            List<Item> Items = new List<Item>();

            lock (Input)
            {
                foreach (int Id in Input.ToList())
                {
                    Item Itm = _room.GetRoomItemHandler().GetItem(Id);
                    if (Itm != null && !Items.Contains(Itm))
                        Items.Add(Itm);
                }
            }

            return Items.ToList();
        }

        public List<Item> GetRoomItemForSquare(int pX, int pY, double minZ)
        {
            var itemsToReturn = new List<Item>();


            var coord = new Point(pX, pY);
            if (_coordinatedItems.ContainsKey(coord))
            {
                var itemsFromSquare = GetItemsFromIds((List<int>)_coordinatedItems[coord]);

                foreach (Item item in itemsFromSquare)
                    if (item.GetZ > minZ)
                        if (item.GetX == pX && item.GetY == pY)
                            itemsToReturn.Add(item);
            }

            return itemsToReturn;
        }

        public List<Item> GetRoomItemForSquare(int pX, int pY)
        {
            var coord = new Point(pX, pY);
            //List<RoomItem> itemsFromSquare = new List<RoomItem>();
            var itemsToReturn = new List<Item>();

            if (_coordinatedItems.ContainsKey(coord))
            {
                var itemsFromSquare = GetItemsFromIds((List<int>)_coordinatedItems[coord]);

                foreach (Item item in itemsFromSquare)
                {
                    if (item.Coordinate.X == coord.X && item.Coordinate.Y == coord.Y)
                        itemsToReturn.Add(item);
                }
            }

            return itemsToReturn;
        }

        public List<Item> GetAllRoomItemForSquare(int pX, int pY)
        {
            Point Coord = new Point(pX, pY);

            List<Item> Items = new List<Item>();
            List<int> Ids;

            // CHANGED THIS ~  IF FAILED CHANGE BACK

            if (_coordinatedItems.TryGetValue(Coord, out Ids))
                Items = GetItemsFromIds(Ids);
            else
                Items = new List<Item>();

            return Items;
        }

        public bool SquareHasUsers(int X, int Y)
        {
            return MapGotUser(new Point(X, Y));
        }


        public static bool TilesTouching(int X1, int Y1, int X2, int Y2)
        {
            if (!(Math.Abs(X1 - X2) > 1 || Math.Abs(Y1 - Y2) > 1)) return true;
            if (X1 == X2 && Y1 == Y2) return true;
            return false;
        }

        public static int TileDistance(int X1, int Y1, int X2, int Y2)
        {
            return Math.Abs(X1 - X2) + Math.Abs(Y1 - Y2);
        }

        public DynamicRoomModel Model
        {
            get { return _dynamicModel; }
        }

        public RoomModel StaticModel
        {
            get { return _model; }
        }

        public byte[,] EffectMap
        {
            get { return _userItemEffect; }
        }

        public byte[,] GameMap
        {
            get { return _gameMap; }
        }

        public void Dispose()
        {
            _userMap.Clear();
            _dynamicModel.Destroy();
            _coordinatedItems.Clear();

            Array.Clear(_gameMap, 0, _gameMap.Length);
            Array.Clear(_userItemEffect, 0, _userItemEffect.Length);
            Array.Clear(_itemHeightMap, 0, _itemHeightMap.Length);

            _userMap = null;
            _gameMap = null;
            _userItemEffect = null;
            _itemHeightMap = null;
            _coordinatedItems = null;

            _dynamicModel = null;
            this._room = null;
            _model = null;
        }
    }
}