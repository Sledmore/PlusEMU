using System.Data;
using System.Collections.Generic;

using log4net;
using Plus.Database.Interfaces;

namespace Plus.Core.Settings
{
    public class SettingsManager
    {
        private Dictionary<string, string> _settings = new Dictionary<string, string>();

        private static readonly ILog log = LogManager.GetLogger("Plus.Core.Settings.SettingsManager");

        public SettingsManager()
        {
            this._settings = new Dictionary<string, string>();
        }

        public void Init()
        {
            if (this._settings.Count > 0)
                this._settings.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_settings`");
                DataTable Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        this._settings.Add(Row["key"].ToString().ToLower(), Row["value"].ToString().ToLower());
                    }
                }
            }

            log.Info("Loaded " + this._settings.Count + " server settings.");
        }

        public string TryGetValue(string value)
        {
            return this._settings.ContainsKey(value) ? this._settings[value] : "0";
        }
    }
}
