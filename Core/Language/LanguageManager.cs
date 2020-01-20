using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Serilog;

namespace Plus.Core.Language
{
    public class LanguageManager
    {
        private readonly Dictionary<string, string> _values;

        public LanguageManager()
        {
            _values = new Dictionary<string, string>();
        }

        public void Init()
        {
            if (_values.Count > 0)
                _values.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_locale`");
                DataTable Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        _values.Add(Row["key"].ToString(), Row["value"].ToString());
                    }
                }
            }

            Log.Information("Loaded " + _values.Count + " language locales.");
        }

        public string TryGetValue(string value)
        {
            return _values.ContainsKey(value) ? _values[value] : "No language locale found for [" + value + "]";
        }
    }
}
