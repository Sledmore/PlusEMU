using System;
using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Catalog.Clothing
{
    public class ClothingManager
    {
        private readonly Dictionary<int, ClothingItem> _clothing;

        public ClothingManager()
        {
            _clothing = new Dictionary<int, ClothingItem>();
        }

        public void Init()
        {
            if (this._clothing.Count > 0)
                this._clothing.Clear();

            DataTable data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`clothing_name`,`clothing_parts` FROM `catalog_clothing`");
                data = dbClient.GetTable();
            }

            if (data != null)
            {
                foreach (DataRow row in data.Rows)
                {
                    _clothing.Add(Convert.ToInt32(row["id"]), new ClothingItem(Convert.ToInt32(row["id"]), Convert.ToString(row["clothing_name"]), Convert.ToString(row["clothing_parts"])));
                }
            }
        }

        public bool TryGetClothing(int itemId, out ClothingItem clothing)
        {
            return _clothing.TryGetValue(itemId, out clothing);
        }

        public ICollection<ClothingItem> GetClothingAllParts
        {
            get { return _clothing.Values; }
        }
    }
}
