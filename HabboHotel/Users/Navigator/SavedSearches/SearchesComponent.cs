using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Users.Navigator.SavedSearches
{
    public class SearchesComponent
    {
        private ConcurrentDictionary<int, SavedSearch> _savedSearches;

        public SearchesComponent()
        {
            this._savedSearches = new ConcurrentDictionary<int, SavedSearch>();
        }

        public bool Init(Habbo habbo)
        {
            if (this._savedSearches.Count > 0)
                this._savedSearches.Clear();

            DataTable GetSearches = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`filter`,`search_code` FROM `user_saved_searches` WHERE `user_id` = @UserId");
                dbClient.AddParameter("UserId", habbo.Id);
                GetSearches = dbClient.GetTable();

                if (GetSearches != null)
                {
                    foreach (DataRow Row in GetSearches.Rows)
                    {
                        this._savedSearches.TryAdd(Convert.ToInt32(Row["id"]), new SavedSearch(Convert.ToInt32(Row["id"]), Convert.ToString(Row["filter"]), Convert.ToString(Row["search_code"])));
                    }
                }
            }
            return true;
        }

        public ICollection<SavedSearch> Searches
        {
            get { return this._savedSearches.Values; }
        }

        public bool TryAdd(int Id, SavedSearch Search)
        {
            return this._savedSearches.TryAdd(Id, Search);
        }

        public bool TryRemove(int Id, out SavedSearch Removed)
        {
            return this._savedSearches.TryRemove(Id, out Removed);
        }
    }
}
