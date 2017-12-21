using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Rooms.Games
{
    public class GameItemHandler
    {
        private Room _room;
        private Random rnd;
        private ConcurrentDictionary<int, Item> _banzaiPyramids;
        private ConcurrentDictionary<int, Item> _banzaiTeleports;

        public GameItemHandler(Room room)
        {
            this._room = room;
            this.rnd = new Random();

            _banzaiPyramids = new ConcurrentDictionary<int, Item>();
            _banzaiTeleports = new ConcurrentDictionary<int, Item>();
        }

        public void OnCycle()
        {
            CyclePyramids();
        }

        private void CyclePyramids()
        {
            Random rnd = new Random();

            foreach (Item item in _banzaiPyramids.Values.ToList())
            {
                if (item == null)
                    continue;

                if (item.interactionCountHelper == 0 && item.ExtraData == "1")
                {
                    _room.GetGameMap().RemoveFromMap(item, false);
                    item.interactionCountHelper = 1;
                }

                if (string.IsNullOrEmpty(item.ExtraData))
                    item.ExtraData = "0";

                int randomNumber = rnd.Next(0, 30);
                if (randomNumber == 15)
                {
                    if (item.ExtraData == "0")
                    {
                        item.ExtraData = "1";
                        item.UpdateState();
                        _room.GetGameMap().RemoveFromMap(item, false);
                    }
                    else
                    {
                        if (_room.GetGameMap().ItemCanBePlaced(item.GetX, item.GetY))
                        {
                            item.ExtraData = "0";
                            item.UpdateState();
                            _room.GetGameMap().AddItemToMap(item);
                        }
                    }
                }
            }
        }

        public void AddPyramid(Item item, int itemId)
        {
            if (_banzaiPyramids.ContainsKey(itemId))
                _banzaiPyramids[itemId] = item;
            else
                _banzaiPyramids.TryAdd(itemId, item);
        }

        public void RemovePyramid(int itemId)
        {
            _banzaiPyramids.TryRemove(itemId, out Item Item);
        }

        public void AddTeleport(Item item, int itemId)
        {
            if (_banzaiTeleports.ContainsKey(itemId))
                _banzaiTeleports[itemId] = item;
            else
                _banzaiTeleports.TryAdd(itemId, item);
        }

        public void RemoveTeleport(int itemId)
        {
            _banzaiTeleports.TryRemove(itemId, out Item Item);
        }

        public void OnTeleportRoomUserEnter(RoomUser user, Item item)
        {
            IEnumerable<Item> items = _banzaiTeleports.Values.Where(p => p.Id != item.Id);

            int count = items.Count();

            int countID = rnd.Next(0, count);
            int countAmount = 0;

            if (count == 0)
                return;

            foreach (Item i in items.ToList())
            {
                if (i == null)
                    continue;

                if (countAmount == countID)
                {
                    i.ExtraData = "1";
                    i.UpdateNeeded = true;

                    _room.GetGameMap().TeleportToItem(user, item);

                    i.ExtraData = "1";
                    i.UpdateNeeded = true;
                    i.UpdateState();
                    i.UpdateState();
                }

                countAmount++;
            }
        }

        public void Dispose()
        {
            if (this._banzaiTeleports != null)
                this._banzaiTeleports.Clear();
            if (this._banzaiPyramids != null)
                this._banzaiPyramids.Clear();
            this._banzaiPyramids = null;
            this._banzaiTeleports = null;
            this._room = null;
            this.rnd = null;
        }
    }
}