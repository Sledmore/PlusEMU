using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.AI
{
    public abstract class BotAI
    {
        public int BaseId;
        private int _roomId;
        private int _roomUserId;
        private Room _room;
        private RoomUser _roomUser;

        public void Init(int baseId, int roomUserId, int roomId, RoomUser user, Room room)
        {
            BaseId = baseId;
            _roomUserId = roomUserId;
            _roomId = roomId;
            _roomUser = user;
            _room = room;
        }

        public Room GetRoom()
        {
            return _room;
        }

        public RoomUser GetRoomUser()
        {
            return _roomUser;
        }

        public RoomBot GetBotData()
        {
            RoomUser user = GetRoomUser();
            if (user == null)
                return null;

            return GetRoomUser().BotData;
        }

        public abstract void OnSelfEnterRoom();
        public abstract void OnSelfLeaveRoom(bool kicked);
        public abstract void OnUserEnterRoom(RoomUser user);
        public abstract void OnUserLeaveRoom(GameClient client);
        public abstract void OnUserSay(RoomUser user, string message);
        public abstract void OnUserShout(RoomUser user, string message);
        public abstract void OnTimerTick();
    }
}