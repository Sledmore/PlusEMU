using System;
using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Plus.HabboHotel.LandingView.Promotions;
using Serilog;

namespace Plus.HabboHotel.LandingView
{
    public class LandingViewManager
    {
        private Dictionary<int, Promotion> _promotionItems;

        public LandingViewManager()
        {
            _promotionItems = new Dictionary<int, Promotion>();
        }

        public void Init()
        {
            if (_promotionItems.Count > 0)
                _promotionItems.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_landing` ORDER BY `id` DESC");
                DataTable GetData = dbClient.GetTable();

                if (GetData != null)
                {
                    foreach (DataRow Row in GetData.Rows)
                    {
                        _promotionItems.Add(Convert.ToInt32(Row[0]), new Promotion((int)Row[0], Row[1].ToString(), Row[2].ToString(), Row[3].ToString(), Convert.ToInt32(Row[4]), Row[5].ToString(), Row[6].ToString()));
                    }
                }
            }


            Log.Information("Landing View Manager -> LOADED");
        }

        public ICollection<Promotion> GetPromotionItems()
        {
            return _promotionItems.Values;
        }
    }
}