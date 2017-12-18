namespace Plus.HabboHotel.Users.Navigator.SavedSearches
{
    public class SavedSearch
    {
        private int _id;
        private string _filter;
        private string _search;

        public SavedSearch(int id, string filter, string search)
        {
            _id = id;
            _filter = filter;
            _search = search;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public string Search
        {
            get { return _search; }
            set { _search = value; }
        }
    }
}
