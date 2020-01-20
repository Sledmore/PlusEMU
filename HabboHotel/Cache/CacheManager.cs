using Plus.Database.Interfaces;
using Plus.HabboHotel.Cache.Process;
using Plus.HabboHotel.GameClients;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Plus.HabboHotel.Cache.Type;
using Serilog;

namespace Plus.HabboHotel.Cache
{
    public class CacheManager
    {
        private readonly ConcurrentDictionary<int, UserCache> _usersCached;
        private readonly ProcessComponent _process;

        public CacheManager()
        {
            _usersCached = new ConcurrentDictionary<int, UserCache>();
            _process = new ProcessComponent();
            _process.Init();
            Log.Information("Cache Manager -> LOADED");
        }
        public bool ContainsUser(int id)
        {
            return _usersCached.ContainsKey(id);
        }

        public UserCache GenerateUser(int id)
        {
            UserCache user = null;

            if (_usersCached.ContainsKey(id))
                if (TryGetUser(id, out user))
                    return user;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(id);
            if (client != null)
                if (client.GetHabbo() != null)
                {
                    user = new UserCache(id, client.GetHabbo().Username, client.GetHabbo().Motto, client.GetHabbo().Look);
                    _usersCached.TryAdd(id, user);
                    return user;
                }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username`, `motto`, `look` FROM users WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", id);

                DataRow dRow = dbClient.GetRow();

                if (dRow != null)
                {
                    user = new UserCache(id, dRow["username"].ToString(), dRow["motto"].ToString(), dRow["look"].ToString());
                    _usersCached.TryAdd(id, user);
                }
            }

            return user;
        }

        public bool TryRemoveUser(int id, out UserCache user)
        {
            return _usersCached.TryRemove(id, out user);
        }

        public bool TryGetUser(int id, out UserCache user)
        {
            return _usersCached.TryGetValue(id, out user);
        }

        public ICollection<UserCache> GetUserCache()
        {
            return _usersCached.Values;
        }
    }
}