using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.GameClients;
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
            this._room = room;
            this.gates = new Item[4];
            this._balls = new ConcurrentDictionary<int, Item>();
            this._gameStarted = false;
        }
        public bool GameIsStarted
        {
            get { return this._gameStarted; }
        }
        public void StopGame(bool userTriggered = false)
        {
            this._gameStarted = false;

            if (!userTriggered)
                _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameEnds, null);
        }

        public void StartGame()
        {
            this._gameStarted = true;
        }

        public void AddBall(Item item)
        {
            this._balls.TryAdd(item.Id, item);
        }

        public void RemoveBall(int itemID)
        {
            Item Item = null;
            this._balls.TryRemove(itemID, out Item);
        }

        public void OnUserWalk(RoomUser User)
        {
            if (User == null)
                return;

            foreach (Item item in this._balls.Values.ToList())
            {
                int NewX = 0;
                int NewY = 0;
                int differenceX = User.X - item.GetX;
                int differenceY = User.Y - item.GetY;

                if (differenceX == 0 && differenceY == 0)
                {
                    if (User.RotBody == 4)
                    {
                        NewX = User.X;
                        NewY = User.Y + 2;

                    }
                    else if (User.RotBody == 6)
                    {
                        NewX = User.X - 2;
                        NewY = User.Y;

                    }
                    else if (User.RotBody == 0)
                    {
                        NewX = User.X;
                        NewY = User.Y - 2;

                    }
                    else if (User.RotBody == 2)
                    {
                        NewX = User.X + 2;
                        NewY = User.Y;

                    }
                    else if (User.RotBody == 1)
                    {
                        NewX = User.X + 2;
                        NewY = User.Y - 2;

                    }
                    else if (User.RotBody == 7)
                    {
                        NewX = User.X - 2;
                        NewY = User.Y - 2;

                    }
                    else if (User.RotBody == 3)
                    {
                        NewX = User.X + 2;
                        NewY = User.Y + 2;

                    }
                    else if (User.RotBody == 5)
                    {
                        NewX = User.X - 2;
                        NewY = User.Y + 2;
                    }

                    if (!this._room.GetRoomItemHandler().CheckPosItem(User.GetClient(), item, NewX, NewY, item.Rotation, false, false))
                    {
                        if (User.RotBody == 0)
                        {
                            NewX = User.X;
                            NewY = User.Y + 1;
                        }
                        else if (User.RotBody == 2)
                        {
                            NewX = User.X - 1;
                            NewY = User.Y;
                        }
                        else if (User.RotBody == 4)
                        {
                            NewX = User.X;
                            NewY = User.Y - 1;
                        }
                        else if (User.RotBody == 6)
                        {
                            NewX = User.X + 1;
                            NewY = User.Y;
                        }
                        else if (User.RotBody == 5)
                        {
                            NewX = User.X + 1;
                            NewY = User.Y - 1;
                        }
                        else if (User.RotBody == 3)
                        {
                            NewX = User.X - 1;
                            NewY = User.Y - 1;
                        }
                        else if (User.RotBody == 7)
                        {
                            NewX = User.X + 1;
                            NewY = User.Y + 1;
                        }
                        else if (User.RotBody == 1)
                        {
                            NewX = User.X - 1;
                            NewY = User.Y + 1;
                        }
                    }
                }
                else if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1 && VerifyBall(User, item.Coordinate.X, item.Coordinate.Y))//VERYFIC BALL CHECAR SI ESTA EN DIRECCION ASIA LA PELOTA
                {
                    NewX = differenceX * -1;
                    NewY = differenceY * -1;

                    NewX = NewX + item.GetX;
                    NewY = NewY + item.GetY;
                }

                if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                {
                    MoveBall(item, NewX, NewY, User);
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
                item.team = TEAM.Blue;
                gates[0] = item;
            }
            else if (gates[1] == null)
            {
                item.team = TEAM.Red;
                gates[1] = item;
            }
            else if (gates[2] == null)
            {
                item.team = TEAM.Green;
                gates[2] = item;
            }
            else if (gates[3] == null)
            {
                item.team = TEAM.Yellow;
                gates[3] = item;
            }
        }

        public void UnRegisterGate(Item item)
        {
            switch (item.team)
            {
                case TEAM.Blue:
                    {
                        gates[0] = null;
                        break;
                    }
                case TEAM.Red:
                    {
                        gates[1] = null;
                        break;
                    }
                case TEAM.Green:
                    {
                        gates[2] = null;
                        break;
                    }
                case TEAM.Yellow:
                    {
                        gates[3] = null;
                        break;
                    }
            }
        }

        public void onGateRemove(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GOAL_RED:
                case InteractionType.footballcounterred:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.Red);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_GREEN:
                case InteractionType.footballcountergreen:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.Green);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_BLUE:
                case InteractionType.footballcounterblue:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.Blue);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_YELLOW:
                case InteractionType.footballcounteryellow:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.Yellow);
                        break;
                    }
            }
        }

        public void MoveBall(Item item, int newX, int newY, RoomUser user)
        {
            if (item == null || user == null)
                return;

            if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY))
                return;

            Point oldRoomCoord = item.Coordinate;
            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
                return;

            double NewZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];

            _room.SendPacket(new SlideObjectBundleComposer(item.Coordinate.X, item.Coordinate.Y, item.GetZ, newX, newY, NewZ, item.Id, item.Id, item.Id));

            item.ExtraData = "11";
            item.UpdateNeeded = true;

            _room.GetRoomItemHandler().SetFloorItem(null, item, newX, newY, item.Rotation, false, false, false, false);

            this._room.OnUserShoot(user, item);
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