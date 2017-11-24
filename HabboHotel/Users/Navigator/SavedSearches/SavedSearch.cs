using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.HabboHotel.Users.Navigator.SavedSearches
{
    public class SavedSearch
    {
        private int _id;
        private string _filter;
        private string _search;

        public SavedSearch(int Id, string Filter, string Search)
        {
            this._id = Id;
            this._filter = Filter;
            this._search = Search;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Filter
        {
            get { return this._filter; }
            set { this._filter = value; }
        }

        public string Search
        {
            get { return this._search; }
            set { this._search = value; }
        }
    }
}
