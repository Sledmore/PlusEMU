using System;
using System.Linq;
using System.Drawing;
using System.Collections.Concurrent;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Items.Wired;
using Plus.HabboHotel.Rooms.PathFinding;

namespace Plus.HabboHotel.Rooms.Games.Football
{
    public class Soccer
    {
        private Room _room;
        private Item[] gates;
        private ConcurrentDictionary<int, Item> _balls;
        private bool _gameStarted;

        public Soccer(Room room)
        {
            _room = room;
            gates = new Item[4];
            _balls = new ConcurrentDictionary<int, Item>();
            _gameStarted = false;
        }

        public bool GameIsStarted
        {
            get { return _gameStarted; }
        }

        public void StopGame(bool triggeredByUser = false)
        {
            _gameStarted = false;

            if (!triggeredByUser)
                _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameEnds, null);
        }

        public void StartGame()
        {
            _gameStarted = true;
        }

        public void AddBall(Item item)
        {
            _balls.TryAdd(item.Id, item);
        }

        public void RemoveBall(int itemId)
        {
            _balls.TryRemove(itemId, out Item Item);
        }

        public void OnUserWalk(RoomUser user)
        {
            if (user == null)
                return;

            foreach (Item item in _balls.Values.ToList())
            {
                int NewX = 0;
                int NewY = 0;
                int differenceX = user.X - item.GetX;
                int differenceY = user.Y - item.GetY;

                if (differenceX == 0 && differenceY == 0)
                {
                    if (user.RotBody == 4)
                    {
                        NewX = user.X;
                        NewY = user.Y + 2;

                    }
                    else if (user.RotBody == 6)
                    {
                        NewX = user.X - 2;
                        NewY = user.Y;

                    }
                    else if (user.RotBody == 0)
                    {
                        NewX = user.X;
                        NewY = user.Y - 2;

                    }
                    else if (user.RotBody == 2)
                    {
                        NewX = user.X + 2;
                        NewY = user.Y;

                    }
                    else if (user.RotBody == 1)
                    {
                        NewX = user.X + 2;
                        NewY = user.Y - 2;

                    }
                    else if (user.RotBody == 7)
                    {
                        NewX = user.X - 2;
                        NewY = user.Y - 2;

                    }
                    else if (user.RotBody == 3)
                    {
                        NewX = user.X + 2;
                        NewY = user.Y + 2;

                    }
                    else if (user.RotBody == 5)
                    {
                        NewX = user.X - 2;
                        NewY = user.Y + 2;
                    }

                    if (!_room.GetRoomItemHandler().CheckPosItem(item, NewX, NewY, item.Rotation))
                    {
                        if (user.RotBody == 0)
                        {
                            NewX = user.X;
                            NewY = user.Y + 1;
                        }
                        else if (user.RotBody == 2)
                        {
                            NewX = user.X - 1;
                            NewY = user.Y;
                        }
                        else if (user.RotBody == 4)
                        {
                            NewX = user.X;
                            NewY = user.Y - 1;
                        }
                        else if (user.RotBody == 6)
                        {
                            NewX = user.X + 1;
                            NewY = user.Y;
                        }
                        else if (user.RotBody == 5)
                        {
                            NewX = user.X + 1;
                            NewY = user.Y - 1;
                        }
                        else if (user.RotBody == 3)
                        {
                            NewX = user.X - 1;
                            NewY = user.Y - 1;
                        }
                        else if (user.RotBody == 7)
                        {
                            NewX = user.X + 1;
                            NewY = user.Y + 1;
                        }
                        else if (user.RotBody == 1)
                        {
                            NewX = user.X - 1;
                            NewY = user.Y + 1;
                        }
                    }
                }
                else if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1 && VerifyBall(user, item.Coordinate.X, item.Coordinate.Y))//VERYFIC BALL CHECAR SI ESTA EN DIRECCION ASIA LA PELOTA
                {
                    NewX = differenceX * -1;
                    NewY = differenceY * -1;

                    NewX = NewX + item.GetX;
                    NewY = NewY + item.GetY;
                }

                if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                {
                    MoveBall(item, NewX, NewY, user);
                }
            }
        }

        private bool VerifyBall(RoomUser user, int actualx, int actualy)
        {
            return Rotation.Calculate(user.X, user.Y, actualx, actualy) == user.RotBody;
        }

        public void RegisterGate(Item item)
        {
            if (gates[0] == null)
            {
                item.team = Team.Blue;
                gates[0] = item;
            }
            else if (gates[1] == null)
            {
                item.team = Team.Red;
                gates[1] = item;
            }
            else if (gates[2] == null)
            {
                item.team = Team.Green;
                gates[2] = item;
            }
            else if (gates[3] == null)
            {
                item.team = Team.Yellow;
                gates[3] = item;
            }
        }

        public void UnRegisterGate(Item item)
        {
            switch (item.team)
            {
                case Team.Blue:
                    {
                        gates[0] = null;
                        break;
                    }
                case Team.Red:
                    {
                        gates[1] = null;
                        break;
                    }
                case Team.Green:
                    {
                        gates[2] = null;
                        break;
                    }
                case Team.Yellow:
                    {
                        gates[3] = null;
                        break;
                    }
            }
        }

        public void OnGateRemove(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GOAL_RED:
                case InteractionType.footballcounterred:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, Team.Red);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_GREEN:
                case InteractionType.footballcountergreen:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, Team.Green);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_BLUE:
                case InteractionType.footballcounterblue:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, Team.Blue);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_YELLOW:
                case InteractionType.footballcounteryellow:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, Team.Yellow);
                        break;
                    }
            }
        }

        public void MoveBall(Item item, int newX, int newY, RoomUser user)
        {
            if (item == null || user == null)
                return;

            if (!_room.GetGameMap().ItemCanBePlaced(newX, newY))
                return;

            Point oldRoomCoord = item.Coordinate;
            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
                return;

            double NewZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];

            _room.SendPacket(new SlideObjectBundleComposer(item.Coordinate.X, item.Coordinate.Y, item.GetZ, newX, newY, NewZ, item.Id, item.Id, item.Id));

            item.ExtraData = "11";
            item.UpdateNeeded = true;

            _room.GetRoomItemHandler().SetFloorItem(null, item, newX, newY, item.Rotation, false, false, false, false);

            _room.OnUserShoot(user, item);
        }

        public void Dispose()
        {
            Array.Clear(gates, 0, gates.Length);
            gates = null;
            _room = null;
            _balls.Clear();
            _balls = null;
        }
    }
}