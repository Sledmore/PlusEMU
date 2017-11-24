using System;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Rooms.AI
{
    public abstract class BotAI
    {
        public int BaseId;
        private int RoomId;
        private int RoomUserId;
        private Room room;
        private RoomUser roomUser;

        public void Init(int pBaseId, int pRoomUserId, int pRoomId, RoomUser user, Room room)
        {
            this.BaseId = pBaseId;
            this.RoomUserId = pRoomUserId;
            this.RoomId = pRoomId;
            this.roomUser = user;
            this.room = room;
        }

        public Room GetRoom()
        {
            return room;
        }

        public RoomUser GetRoomUser()
        {
            return roomUser;
        }

        public RoomBot GetBotData()
        {
            RoomUser User = GetRoomUser();
            if (User == null)
                return null;
            else
                return GetRoomUser().BotData;
        }

        public abstract void OnSelfEnterRoom();
        public abstract void OnSelfLeaveRoom(bool Kicked);
        public abstract void OnUserEnterRoom(RoomUser User);
        public abstract void OnUserLeaveRoom(GameClient Client);
        public abstract void OnUserSay(RoomUser User, string Message);
        public abstract void OnUserShout(RoomUser User, string Message);
        public abstract void OnTimerTick();
    }
}