using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;

using Plus.Communication.Packets.Incoming;

using Plus.Utilities.Enclosure;
using System.Linq;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using System.Collections.Concurrent;
using Plus.HabboHotel.Items.Wired;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Rooms.PathFinding;

namespace Plus.HabboHotel.Rooms.Games.Banzai
{
    public class BattleBanzai
    {
        private Room _room;
        private byte[,] floorMap;
        private double timestarted;
        private bool banzaiStarted;
        private GameField field;
        private ConcurrentDictionary<int, Item> _pucks;
        private ConcurrentDictionary<int, Item> _banzaiTiles;

        public BattleBanzai(Room room)
        {
            this._room = room;
            banzaiStarted = false;
            timestarted = 0;
            this._pucks = new ConcurrentDictionary<int, Item>();
            this._banzaiTiles = new ConcurrentDictionary<int, Item>();
        }

        public bool isBanzaiActive
        {
            get { return banzaiStarted; }
        }

        public void AddTile(Item item, int itemID)
        {
            if (!_banzaiTiles.ContainsKey(itemID))
                _banzaiTiles.TryAdd(itemID, item);
        }

        public void RemoveTile(int itemID)
        {
            Item Item = null;
            _banzaiTiles.TryRemove(itemID, out Item);
        }

        public void AddPuck(Item item)
        {
            if (!_pucks.ContainsKey(item.Id))
                _pucks.TryAdd(item.Id, item);
        }

        public void RemovePuck(int itemID)
        {
            Item Item = null;
            _pucks.TryRemove(itemID, out Item);
        }

        public void OnUserWalk(RoomUser User)
        {
            if (User == null)
                return;

            foreach (Item item in _pucks.Values.ToList())
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
                else if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1 && VerifyPuck(User, item.Coordinate.X, item.Coordinate.Y))//VERYFIC BALL CHECAR SI ESTA EN DIRECCION ASIA LA PELOTA
                {
                    NewX = differenceX * -1;
                    NewY = differenceY * -1;

                    NewX = NewX + item.GetX;
                    NewY = NewY + item.GetY;
                }

                if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                {
                    MovePuck(item, User.GetClient(), NewX, NewY, User.Team);
                }
            }

            if (banzaiStarted)
            {
                HandleBanzaiTiles(User.Coordinate, User.Team, User);
            }
        }

        private bool VerifyPuck(RoomUser user, int actualx, int actualy)
        {
            return Rotation.Calculate(user.X, user.Y, actualx, actualy) == user.RotBody;
        }

        public void BanzaiStart()
        {
            if (banzaiStarted)
                return;

            floorMap = new byte[_room.GetGameMap().Model.MapSizeY, _room.GetGameMap().Model.MapSizeX];
            field = new GameField(floorMap, true);
            timestarted = PlusEnvironment.GetUnixTimestamp();
            _room.GetGameManager().LockGates();
            for (int i = 1; i < 5; i++)
            {
                _room.GetGameManager().Points[i] = 0;
            }

            foreach (Item tile in _banzaiTiles.Values)
            {
                tile.ExtraData = "1";
                tile.value = 0;
                tile.team = TEAM.None;
                tile.UpdateState();
            }

            this.ResetTiles();
            banzaiStarted = true;

            _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameStarts, null);

            foreach (RoomUser user in _room.GetRoomUserManager().GetRoomUsers())
            {
                user.LockedTilesCount = 0;
            }
        }

        public void ResetTiles()
        {
            foreach (Item item in _room.GetRoomItemHandler().GetFloor.ToList())
            {
                InteractionType type = item.GetBaseItem().InteractionType;

                switch (type)
                {
                    case InteractionType.banzaiscoreblue:
                    case InteractionType.banzaiscoregreen:
                    case InteractionType.banzaiscorered:
                    case InteractionType.banzaiscoreyellow:
                        {
                            item.ExtraData = "0";
                            item.UpdateState();
                            break;
                        }
                }
            }
        }

        public void BanzaiEnd(bool userTriggered = false)
        {
            //TODO
            banzaiStarted = false;
            _room.GetGameManager().StopGame();
            floorMap = null;

            if (!userTriggered)
                _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameEnds, null);

            TEAM winners = _room.GetGameManager().GetWinningTeam();
            _room.GetGameManager().UnlockGates();
            foreach (Item tile in _banzaiTiles.Values)
            {
                if (tile.team == winners)
                {
                    tile.interactionCount = 0;
                    tile.interactionCountHelper = 0;
                    tile.UpdateNeeded = true;
                }
                else if (tile.team == TEAM.None)
                {
                    tile.ExtraData = "0";
                    tile.UpdateState();
                }
            }

            if (winners != TEAM.None)
            {
                List<RoomUser> Winners = _room.GetRoomUserManager().GetRoomUsers();

                foreach (RoomUser User in Winners.ToList())
                {
                    if (User.Team != TEAM.None)
                    {
                        if (PlusEnvironment.GetUnixTimestamp() - timestarted > 5)
                        {
                            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallTilesLocked", User.LockedTilesCount);
                            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallPlayer", 1);
                        }
                    }
                    if (winners == TEAM.Blue)
                    {
                        if (User.CurrentEffect == 35)
                        {
                            if (PlusEnvironment.GetUnixTimestamp() - timestarted > 5)
                                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallWinner", 1);
                            _room.SendPacket(new ActionComposer(User.VirtualId, 1));
                        }
                    }
                    else if (winners == TEAM.Red)
                    {
                        if (User.CurrentEffect == 33)
                        {
                            if (PlusEnvironment.GetUnixTimestamp() - timestarted > 5)
                                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallWinner", 1);
                            _room.SendPacket(new ActionComposer(User.VirtualId, 1));
                        }
                    }
                    else if (winners == TEAM.Green)
                    {
                        if (User.CurrentEffect == 34)
                        {
                            if (PlusEnvironment.GetUnixTimestamp() - timestarted > 5)
                                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallWinner", 1);
                            _room.SendPacket(new ActionComposer(User.VirtualId, 1));
                        }
                    }
                    else if (winners == TEAM.Yellow)
                    {
                        if (User.CurrentEffect == 36)
                        {
                            if (PlusEnvironment.GetUnixTimestamp() - timestarted > 5)
                                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallWinner", 1);
                            _room.SendPacket(new ActionComposer(User.VirtualId, 1));
                        }
                    }
                }
                if (field != null)
                    field.Dispose();
            }
        }

        public void MovePuck(Item item, GameClient mover, int newX, int newY, TEAM team)
        {
            if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY))
                return;

            Point oldRoomCoord = item.Coordinate;


            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
                return;

            item.ExtraData = (Convert.ToInt32(team).ToString());
            item.UpdateNeeded = true;
            item.UpdateState();

            Double NewZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];

            _room.SendPacket(new SlideObjectBundleComposer(item.GetX, item.GetY, item.GetZ, newX, newY, NewZ, 0, 0, item.Id));

            _room.GetRoomItemHandler().SetFloorItem(mover, item, newX, newY, item.Rotation, false, false, false, false);

            if (mover == null || mover.GetHabbo() == null)
                return;

            RoomUser user = mover.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(mover.GetHabbo().Id);
            if (banzaiStarted)
            {
                HandleBanzaiTiles(new Point(newX, newY), team, user);
            }

        }

        private void SetTile(Item item, TEAM team, RoomUser user)
        {
            if (item.team == team)
            {
                if (item.value < 3)
                {
                    item.value++;
                    if (item.value == 3)
                    {
                        user.LockedTilesCount++;
                        _room.GetGameManager().AddPointToTeam(item.team, 1);
                        field.updateLocation(item.GetX, item.GetY, (byte)team);
                        List<PointField> gfield = field.doUpdate();
                        TEAM t;
                        foreach (PointField gameField in gfield)
                        {
                            t = (TEAM)gameField.forValue;
                            foreach (Point p in gameField.getPoints())
                            {
                                HandleMaxBanzaiTiles(new Point(p.X, p.Y), t);
                                floorMap[p.Y, p.X] = gameField.forValue;
                            }
                        }
                    }
                }
            }
            else
            {
                if (item.value < 3)
                {
                    item.team = team;
                    item.value = 1;
                }
            }


            int newColor = item.value + (Convert.ToInt32(item.team) * 3) - 1;
            item.ExtraData = newColor.ToString();
        }

        private void HandleBanzaiTiles(Point coord, TEAM team, RoomUser user)
        {
            if (team == TEAM.None)
                return;

            List<Item> items = _room.GetGameMap().GetCoordinatedItems(coord);
            int i = 0;
            foreach (Item _item in _banzaiTiles.Values.ToList())
            {
                if (_item == null)
                    continue;

                if (_item.GetBaseItem().InteractionType != InteractionType.banzaifloor)
                {
                    user.Team = TEAM.None;
                    user.ApplyEffect(0);
                    continue;
                }

                if (_item.ExtraData.Equals("5") || _item.ExtraData.Equals("8") || _item.ExtraData.Equals("11") ||
                    _item.ExtraData.Equals("14"))
                {
                    i++;
                    continue;
                }

                if (_item.GetX != coord.X || _item.GetY != coord.Y)
                    continue;

                SetTile(_item, team, user);
                if (_item.ExtraData.Equals("5") || _item.ExtraData.Equals("8") || _item.ExtraData.Equals("11") ||
                    _item.ExtraData.Equals("14"))
                    i++;
                _item.UpdateState(false, true);
            }
            if (i == _banzaiTiles.Count)
                BanzaiEnd();
        }

        private void HandleMaxBanzaiTiles(Point coord, TEAM team)
        {
            if (team == TEAM.None)
                return;

            List<Item> items = _room.GetGameMap().GetCoordinatedItems(coord);

            foreach (Item _item in _banzaiTiles.Values.ToList())
            {
                if (_item == null)
                    continue;

                if (_item.GetBaseItem().InteractionType != InteractionType.banzaifloor)
                    continue;

                if (_item.GetX != coord.X || _item.GetY != coord.Y)
                    continue;

                SetMaxForTile(_item, team);
                _room.GetGameManager().AddPointToTeam(team, 1);
                _item.UpdateState(false, true);
            }
        }

        private static void SetMaxForTile(Item item, TEAM team)
        {
            if (item.value < 3)
            {
                item.value = 3;
                item.team = team;
            }

            int newColor = item.value + (Convert.ToInt32(item.team) * 3) - 1;
            item.ExtraData = newColor.ToString();
        }

        public void Dispose()
        {
            _banzaiTiles.Clear();
            _pucks.Clear();

            if (floorMap != null)
                Array.Clear(floorMap, 0, floorMap.Length);

            if (field != null)
                field.Dispose();

            _room = null;
            _banzaiTiles = null;
            _pucks = null;
            floorMap = null;
            field = null;
        }
    }
}