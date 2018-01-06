using System.Data;
using System.Collections.Generic;

using log4net;
using Plus.Database.Interfaces;

namespace Plus.Core.Settings
{
    public class SettingsManager
    {
        private readonly Dictionary<string, string> _settings;

        private static readonly ILog Log = LogManager.GetLogger("Plus.Core.Settings.SettingsManager");

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

            Log.Info("Loaded " + _settings.Count + " server settings.");
        }

        public string TryGetValue(string value)
        {
            return _settings.ContainsKey(value) ? _settings[value] : "0";
        }
    }
}
