using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Serilog;

namespace Plus.HabboHotel.Games
{
    public class GameDataManager
    {
        private readonly Dictionary<int, GameData> _games;

        public GameDataManager()
        {
            _games = new Dictionary<int, GameData>();
        }

        public void Init()
        {
            if (_games.Count > 0)
                _games.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable data = null;
                dbClient.SetQuery("SELECT `id`,`name`,`colour_one`,`colour_two`,`resource_path`,`string_three`,`game_swf`,`game_assets`,`game_server_host`,`game_server_port`,`socket_policy_port`,`game_enabled` FROM `games_config`");
                data = dbClient.GetTable();

                if (data != null)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        _games.Add(Convert.ToInt32(row["id"]), new GameData(Convert.ToInt32(row["id"]), Convert.ToString(row["name"]), Convert.ToString(row["colour_one"]), Convert.ToString(row["colour_two"]), Convert.ToString(row["resource_path"]), Convert.ToString(row["string_three"]), Convert.ToString(row["game_swf"]), Convert.ToString(row["game_assets"]), Convert.ToString(row["game_server_host"]), Convert.ToString(row["game_server_port"]), Convert.ToString(row["socket_policy_port"]), PlusEnvironment.EnumToBool(row["game_enabled"].ToString())));
                    }
                }
            }

            Log.Information("Game Data Manager -> LOADED");
        }

        public bool TryGetGame(int gameId, out GameData data)
        {
            return _games.TryGetValue(gameId, out data);
        }

        public int GetCount()
        {
            return _games.Values.Count(x => x.Enabled);
        }
    
        public ICollection<GameData> GameData
        {
            get
            {
                return _games.Values;
            }
        }
    }
}