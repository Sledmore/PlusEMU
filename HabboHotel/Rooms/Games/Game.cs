using System;
using System.Linq;
using System.Drawing;
using System.Collections.Concurrent;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Games.Teams;

namespace Plus.HabboHotel.Rooms.Games
{
    public class GameManager
    {
        private Room _room;
        private int[] _teamPoints;
        private ConcurrentDictionary<int, Item> _blueTeamItems;
        private ConcurrentDictionary<int, Item> _greenTeamItems;
        private ConcurrentDictionary<int, Item> _redTeamItems;
        private ConcurrentDictionary<int, Item> _yellowTeamItems;

        public GameManager(Room room)
        {
            this._room = room;
            this._teamPoints = new int[5];

            this._redTeamItems = new ConcurrentDictionary<int, Item>();
            this._blueTeamItems = new ConcurrentDictionary<int, Item>();
            this._greenTeamItems = new ConcurrentDictionary<int, Item>();
            this._yellowTeamItems = new ConcurrentDictionary<int, Item>();
        }

        public int[] Points
        {
            get { return this._teamPoints; }
            set { this._teamPoints = value; }
        }

        public TEAM GetWinningTeam()
        {
            int winning = 1;
            int highestScore = 0;

            for (int i = 1; i < 5; i++)
            {
                if (_teamPoints[i] > highestScore)
                {
                    highestScore = _teamPoints[i];
                    winning = i;
                }
            }
            return (TEAM)winning;
        }

        public void AddPointToTeam(TEAM team, int points)
        {
            int newPoints = this._teamPoints[Convert.ToInt32(team)] += points;
            if (newPoints < 0)
                newPoints = 0;

            this._teamPoints[Convert.ToInt32(team)] = newPoints;

            foreach (Item item in GetFurniItems(team).Values.ToList())
            {
                if (!IsFootballGoal(item.GetBaseItem().InteractionType))
                {
                    item.ExtraData = this._teamPoints[Convert.ToInt32(team)].ToString();
                    item.UpdateState();
                }
            }

            foreach (Item item in _room.GetRoomItemHandler().GetFloor.ToList())
            {
                if (team == TEAM.Blue && item.Data.InteractionType == InteractionType.banzaiscoreblue)
                {
                    item.ExtraData = _teamPoints[Convert.ToInt32(team)].ToString();
                    item.UpdateState();
                }
                else if (team == TEAM.Red && item.Data.InteractionType == InteractionType.banzaiscorered)
                {
                    item.ExtraData = _teamPoints[Convert.ToInt32(team)].ToString();
                    item.UpdateState();
                }
                else if (team == TEAM.Green && item.Data.InteractionType == InteractionType.banzaiscoregreen)
                {
                    item.ExtraData = _teamPoints[Convert.ToInt32(team)].ToString();
                    item.UpdateState();
                }
                else if (team == TEAM.Yellow && item.Data.InteractionType == InteractionType.banzaiscoreyellow)
                {
                    item.ExtraData = _teamPoints[Convert.ToInt32(team)].ToString();
                    item.UpdateState();
                }
            }
        }

        public void Reset()
        {
            AddPointToTeam(TEAM.Blue, GetScoreForTeam(TEAM.Blue) * (-1));
            AddPointToTeam(TEAM.Green, GetScoreForTeam(TEAM.Green) * (-1));
            AddPointToTeam(TEAM.Red, GetScoreForTeam(TEAM.Red) * (-1));
            AddPointToTeam(TEAM.Yellow, GetScoreForTeam(TEAM.Yellow) * (-1));
        }

        private int GetScoreForTeam(TEAM team)
        {
            return _teamPoints[Convert.ToInt32(team)];
        }

        private ConcurrentDictionary<int, Item> GetFurniItems(TEAM team)
        {
            switch (team)
            {
                default:
                    return new ConcurrentDictionary<int, Item>();
                case TEAM.Blue:
                    return this._blueTeamItems;
                case TEAM.Green:
                    return this._greenTeamItems;
                case TEAM.Red:
                    return this._redTeamItems;
                case TEAM.Yellow:
                    return this._yellowTeamItems;
            }
        }

        private static bool IsFootballGoal(InteractionType type)
        {
            return (type == InteractionType.FOOTBALL_GOAL_BLUE || type == InteractionType.FOOTBALL_GOAL_GREEN || type == InteractionType.FOOTBALL_GOAL_RED || type == InteractionType.FOOTBALL_GOAL_YELLOW);
        }

        public void AddFurnitureToTeam(Item item, TEAM team)
        {
            switch (team)
            {
                case TEAM.Blue:
                    _blueTeamItems.TryAdd(item.Id, item);
                    break;
                case TEAM.Green:
                    _greenTeamItems.TryAdd(item.Id, item);
                    break;
                case TEAM.Red:
                    _redTeamItems.TryAdd(item.Id, item);
                    break;
                case TEAM.Yellow:
                    _yellowTeamItems.TryAdd(item.Id, item);
                    break;
            }
        }

        public void RemoveFurnitureFromTeam(Item item, TEAM team)
        {
            switch (team)
            {
                case TEAM.Blue:
                    _blueTeamItems.TryRemove(item.Id, out item);
                    break;
                case TEAM.Green:
                    _greenTeamItems.TryRemove(item.Id, out item);
                    break;
                case TEAM.Red:
                    _redTeamItems.TryRemove(item.Id, out item);
                    break;
                case TEAM.Yellow:
                    _yellowTeamItems.TryRemove(item.Id, out item);
                    break;
            }
        }

        #region Gates
        public void LockGates()
        {
            foreach (Item item in this._redTeamItems.Values.ToList())
            {
                LockGate(item);
            }

            foreach (Item item in this._greenTeamItems.Values.ToList())
            {
                LockGate(item);
            }

            foreach (Item item in this._blueTeamItems.Values.ToList())
            {
                LockGate(item);
            }

            foreach (Item item in this._yellowTeamItems.Values.ToList())
            {
                LockGate(item);
            }
        }

        public void UnlockGates()
        {
            foreach (Item item in this._redTeamItems.Values.ToList())
            {
                UnlockGate(item);
            }

            foreach (Item item in this._greenTeamItems.Values.ToList())
            {
                UnlockGate(item);
            }

            foreach (Item item in this._blueTeamItems.Values.ToList())
            {
                UnlockGate(item);
            }

            foreach (Item item in this._yellowTeamItems.Values.ToList())
            {
                UnlockGate(item);
            }
        }

        private void LockGate(Item item)
        {
            InteractionType type = item.GetBaseItem().InteractionType;
            if (type == InteractionType.FREEZE_BLUE_GATE || type == InteractionType.FREEZE_GREEN_GATE ||
                type == InteractionType.FREEZE_RED_GATE || type == InteractionType.FREEZE_YELLOW_GATE
                || type == InteractionType.banzaigateblue || type == InteractionType.banzaigatered ||
                type == InteractionType.banzaigategreen || type == InteractionType.banzaigateyellow)
            {
                foreach (RoomUser user in _room.GetGameMap().GetRoomUsers(new Point(item.GetX, item.GetY)))
                {
                    user.SqState = 0;
                }
                _room.GetGameMap().GameMap[item.GetX, item.GetY] = 0;
            }
        }

        private void UnlockGate(Item item)
        {
            InteractionType type = item.GetBaseItem().InteractionType;
            if (type == InteractionType.FREEZE_BLUE_GATE || type == InteractionType.FREEZE_GREEN_GATE ||
                type == InteractionType.FREEZE_RED_GATE || type == InteractionType.FREEZE_YELLOW_GATE
                || type == InteractionType.banzaigateblue || type == InteractionType.banzaigatered ||
                type == InteractionType.banzaigategreen || type == InteractionType.banzaigateyellow)
            {
                foreach (RoomUser user in _room.GetGameMap().GetRoomUsers(new Point(item.GetX, item.GetY)))
                {
                    user.SqState = 1;
                }
                _room.GetGameMap().GameMap[item.GetX, item.GetY] = 1;
            }
        }
        #endregion

        public void StopGame()
        {
            this._room.lastTimerReset = DateTime.Now;
        }

        public void Dispose()
        {
            Array.Clear(_teamPoints, 0, _teamPoints.Length);
            _redTeamItems.Clear();
            _blueTeamItems.Clear();
            _greenTeamItems.Clear();
            _yellowTeamItems.Clear();

            _teamPoints = null;
            _redTeamItems = null;
            _blueTeamItems = null;
            _greenTeamItems = null;
            _yellowTeamItems = null;
            _room = null;
        }
    }
}