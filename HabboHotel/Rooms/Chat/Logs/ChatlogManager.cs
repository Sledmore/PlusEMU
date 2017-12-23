using System.Threading;
using System.Collections.Generic;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms.Chat.Logs
{
    public sealed class ChatlogManager
    {
        private const int FLUSH_ON_COUNT = 10;

        private readonly List<ChatlogEntry> _chatlogs;
        private readonly ReaderWriterLockSlim _lock;

        public ChatlogManager()
        {
            this._chatlogs = new List<ChatlogEntry>();
            this._lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        public void StoreChatlog(ChatlogEntry Entry)
        {
            this._lock.EnterUpgradeableReadLock();

            this._chatlogs.Add(Entry);

            this.OnChatlogStore();

            this._lock.ExitUpgradeableReadLock();
        }

        private void OnChatlogStore()
        {
            if (this._chatlogs.Count >= FLUSH_ON_COUNT)
                this.FlushAndSave();
        }

        public void FlushAndSave()
        {
            this._lock.EnterWriteLock();

            if (this._chatlogs.Count > 0)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    foreach (ChatlogEntry Entry in this._chatlogs)
                    {
                        dbClient.SetQuery("INSERT INTO chatlogs (`user_id`, `room_id`, `timestamp`, `message`) VALUES " + "(@uid, @rid, @time, @msg)");
                        dbClient.AddParameter("uid", Entry.PlayerId);
                        dbClient.AddParameter("rid", Entry.RoomId);
                        dbClient.AddParameter("time", Entry.Timestamp);
                        dbClient.AddParameter("msg", Entry.Message);
                        dbClient.RunQuery();
                    }
                }
            }

            this._chatlogs.Clear();
            this._lock.ExitWriteLock();
        }
    }
}
