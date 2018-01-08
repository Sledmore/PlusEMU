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
            _chatlogs = new List<ChatlogEntry>();
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        public void StoreChatlog(ChatlogEntry Entry)
        {
            _lock.EnterUpgradeableReadLock();

            _chatlogs.Add(Entry);

            OnChatlogStore();

            _lock.ExitUpgradeableReadLock();
        }

        private void OnChatlogStore()
        {
            if (_chatlogs.Count >= FLUSH_ON_COUNT)
                FlushAndSave();
        }

        public void FlushAndSave()
        {
            _lock.EnterWriteLock();

            if (_chatlogs.Count > 0)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    foreach (ChatlogEntry Entry in _chatlogs)
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

            _chatlogs.Clear();
            _lock.ExitWriteLock();
        }
    }
}
