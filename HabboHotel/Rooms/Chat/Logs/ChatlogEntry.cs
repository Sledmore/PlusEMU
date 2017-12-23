using System;

using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Rooms.Chat.Logs
{
    public sealed class ChatlogEntry
    {
        private int _playerId;
        private int _roomId;
        private string _message;
        private double _timestamp;

        private WeakReference _playerReference;
        private WeakReference _roomReference;

        public ChatlogEntry(int PlayerId, int RoomId, string Message, double Timestamp, Habbo Player = null, RoomData Instance = null)
        {
            this._playerId = PlayerId;
            this._roomId = RoomId;
            this._message = Message;
            this._timestamp = Timestamp;

            if (Player != null)
                this._playerReference = new WeakReference(Player);

            if (Instance != null)
                this._roomReference = new WeakReference(Instance);
        }

        public int PlayerId
        {
            get { return this._playerId; }
        }

        public int RoomId
        {
            get { return this._roomId; }
        }

        public string Message
        {
            get { return this._message; }
        }

        public double Timestamp
        {
            get { return this._timestamp; }
        }

        public Habbo PlayerNullable()
        {
            if (this._playerReference.IsAlive)
            {
                Habbo PlayerObj = (Habbo)this._playerReference.Target;

                return PlayerObj;
            }

            return null;
        }

        public Room RoomNullable()
        {
            if (this._roomReference.IsAlive)
            {
                Room RoomObj = (Room)this._roomReference.Target;
                if (RoomObj.mDisposed)
                    return null;
                return RoomObj;
            }
            return null;
        }
    }
}
