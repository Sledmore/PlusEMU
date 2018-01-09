namespace Plus.HabboHotel.Navigator
{
    public class TopLevelItem
    {
        private int _id;
        private string _searchCode;
        private string _filter;
        private string _localization;

        public TopLevelItem(int id, string searchCode, string filter, string localization)
        {
            _id = id;
            _searchCode = searchCode;
            _filter = filter;
            _localization = localization;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string SearchCode
        {
            get { return _searchCode; }
            set { _searchCode = value; }
        }

        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public string Localization
        {
            get { return _localization; }
            set { _localization = value; }
        }
    }
}