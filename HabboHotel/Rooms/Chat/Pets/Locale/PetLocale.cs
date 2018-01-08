using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Rooms.Chat.Pets.Locale
{
    public class PetLocale
    {
        private Dictionary<string, string[]> _values;

        public PetLocale()
        {
            this._values = new Dictionary<string, string[]>();

            this.Init();
        }

        public void Init()
        {
            this._values = new Dictionary<string, string[]>();
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `bots_pet_responses`");
                DataTable Pets = dbClient.GetTable();

                if (Pets != null)
                {
                    foreach (DataRow Row in Pets.Rows)
                    {
                        this._values.Add(Row[0].ToString(), Row[1].ToString().Split(';'));
                    }
                }
            }
        }

        public string[] GetValue(string key)
        {
            string[] value;
            if (this._values.TryGetValue(key, out value))
                return value;
            return new[] { "Unknown pet speach:" + key };
        }
    }
}