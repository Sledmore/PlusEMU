using System.Collections.Generic;

using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Rooms.Trading
{
    public sealed class TradeUser
    {
        private RoomUser _user;
        private bool _accepted;
        private Dictionary<int, Item> _offeredItems;

        public TradeUser(RoomUser User)
        {
            this._user = User;
            this._accepted = false;
            this._offeredItems = new Dictionary<int, Item>();
        }

        public RoomUser RoomUser
        {
            get { return this._user; }
        }

        public bool HasAccepted
        {
            get { return this._accepted; }
            set { this._accepted = value; }
        }

        public Dictionary<int, Item> OfferedItems
        {
            get { return this._offeredItems; }
            set { this._offeredItems = value; }
        }
    }
}
