using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.HabboHotel.Navigator
{
    public class TopLevelItem
    {
        private int _id;
        private string _searchCode;
        private string _filter;
        private string _localization;

        public TopLevelItem(int Id, string SearchCode, string Filter, string Localization)
        {
            this._id = Id;
            this._searchCode = SearchCode;
            this._filter = Filter;
            this._localization = Localization;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string SearchCode
        {
            get { return this._searchCode; }
            set { this._searchCode = value; }
        }

        public string Filter
        {
            get { return this._filter; }
            set { this._filter = value; }
        }

        public string Localization
        {
            get { return this._localization; }
            set { this._localization = value; }
        }
    }
}