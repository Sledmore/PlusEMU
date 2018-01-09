using System;
using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;

using log4net;

namespace Plus.HabboHotel.Items.Televisions
{
    public class TelevisionManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Items.Televisions.TelevisionManager");

        public Dictionary<int, TelevisionItem> _televisions;

        public TelevisionManager()
        {
            _televisions =  new Dictionary<int, TelevisionItem>();
        }

        public void Init()
        {
            if (_televisions.Count > 0)
                _televisions.Clear();

            DataTable getData = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor()) 
            {
                dbClient.SetQuery("SELECT * FROM `items_youtube` ORDER BY `id` DESC");
                getData = dbClient.GetTable();

                if (getData != null)
                {
                    foreach (DataRow Row in getData.Rows)
                    {
                        _televisions.Add(Convert.ToInt32(Row["id"]), new TelevisionItem(Convert.ToInt32(Row["id"]), Row["youtube_id"].ToString(), Row["title"].ToString(), Row["description"].ToString(), PlusEnvironment.EnumToBool(Row["enabled"].ToString())));
                    }
                }
            }


            log.Info("Television Items -> LOADED");
        }


        public ICollection<TelevisionItem> TelevisionList
        {
            get
            {
                return _televisions.Values;
            }
        }

        public bool TryGet(int ItemId, out TelevisionItem TelevisionItem)
        {
            if (_televisions.TryGetValue(ItemId, out TelevisionItem))
                return true;
            return false;
        }
    }
}
