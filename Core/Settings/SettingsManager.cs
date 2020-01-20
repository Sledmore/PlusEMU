using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Serilog;

namespace Plus.Core.Settings
{
    public class SettingsManager
    {
        private readonly Dictionary<string, string> _settings;

        public SettingsManager()
        {
            _settings = new Dictionary<string, string>();
        }

        public void Init()
        {
            if (_settings.Count > 0)
                _settings.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_settings`");
                DataTable table = dbClient.GetTable();

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        _settings.Add(row["key"].ToString().ToLower(), row["value"].ToString().ToLower());
                    }
                }
            }

            Log.Information("Loaded " + _settings.Count + " server settings.");
        }

        public string TryGetValue(string value)
        {
            return _settings.ContainsKey(value) ? _settings[value] : "0";
        }
    }
}
