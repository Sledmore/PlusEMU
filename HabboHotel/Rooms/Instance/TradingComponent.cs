using System.Collections.Concurrent;

using Plus.HabboHotel.Rooms.Trading;

namespace Plus.HabboHotel.Rooms.Instance
{
    public class TradingComponent
    {
        private int _currentId;
        private Room _instance;
        private readonly ConcurrentDictionary<int, Trade> _activeTrades;

        public TradingComponent(Room Instance)
        {
            _currentId = 1;
            _instance = Instance;
            _activeTrades = new ConcurrentDictionary<int, Trade>();
        }

        public bool StartTrade(RoomUser Player1, RoomUser Player2, out Trade Trade)
        {
            _currentId++;
            Trade = new Trade(_currentId, Player1, Player2, _instance);
            return _activeTrades.TryAdd(_currentId, Trade);
        }

        public bool TryGetTrade(int TradeId, out Trade Trade)
        {
            return _activeTrades.TryGetValue(TradeId, out Trade);
        }

        public bool RemoveTrade(int Id)
        {
            Trade Trade = null;
            return _activeTrades.TryRemove(Id, out Trade);
        }

        public void Cleanup()
        {
            foreach (Trade Trade in _activeTrades.Values)
            {
                foreach (TradeUser User in Trade.Users)
                {
                    if (User == null || User.RoomUser == null)
                        continue;

                    Trade.EndTrade(User.RoomUser.HabboId);
                }
            }
        }
    }
}