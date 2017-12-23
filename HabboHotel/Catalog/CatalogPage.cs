using System.Collections.Generic;

namespace Plus.HabboHotel.Catalog
{
    public class CatalogPage
    {
        private int _id;
        private int _parentId;
        private string _caption;
        private string _pageLink;
        private int _icon;
        private int _minRank;
        private int _minVIP;
        private bool _visible;
        private bool _enabled;
        private string _template;

        private List<string> _pageStrings1;
        private List<string> _pageStrings2;

        private Dictionary<int, CatalogItem> _items;
        private Dictionary<int, CatalogItem> _itemOffers;

        public CatalogPage(int id, int parentId, string enabled, string caption, string pageLink, int icon, int minRank, int minVIP,
              string visible, string template, string pageStrings1, string pageStrings2, Dictionary<int, CatalogItem> items, ref Dictionary<int, int> flatOffers)
        {
            _id = id;
            _parentId = parentId;
            _enabled = enabled.ToLower() == "1" ? true : false;
            _caption = caption;
            _pageLink = pageLink;
            _icon = icon;
            _minRank = minRank;
            _minVIP = minVIP;
            _visible = visible.ToLower() == "1" ? true : false;
            _template = template;

            foreach (string str in pageStrings1.Split('|'))
            {
                if (_pageStrings1 == null) { _pageStrings1 = new List<string>(); }
                _pageStrings1.Add(str);
            }

            foreach (string str in pageStrings2.Split('|'))
            {
                if (_pageStrings2 == null) { _pageStrings2 = new List<string>(); }
                _pageStrings2.Add(str);
            }

            _items = items;

            _itemOffers = new Dictionary<int, CatalogItem>();
            foreach (int i in flatOffers.Keys)
            {
                if (flatOffers[i] == id)
                {
                    foreach (CatalogItem item in _items.Values)
                    {
                        if (item.OfferId == i)
                        {
                            if (!_itemOffers.ContainsKey(i))
                                _itemOffers.Add(i, item);
                        }
                    }
                }
            }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string PageLink
        {
            get { return _pageLink; }
            set { _pageLink = value; }
        }

        public int Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        public int MinimumRank
        {
            get { return _minRank; }
            set { _minRank = value; }
        }

        public int MinimumVIP
        {
            get { return _minVIP;}
            set { _minVIP = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public string Template
        {
            get { return _template; }
            set { _template = value; }
        }

        public List<string> PageStrings1
        {
            get { return _pageStrings1; }
            private set { _pageStrings1 = value; }
        }

        public List<string> PageStrings2
        {
            get { return _pageStrings2; }
            private set { _pageStrings2 = value; }
        }

        public Dictionary<int, CatalogItem> Items
        {
            get { return _items; }
            private set { _items = value; }
        }
        
        public Dictionary<int, CatalogItem> ItemOffers
        {
            get { return _itemOffers; }
            private set { _itemOffers = value; }
        }

        public CatalogItem GetItem(int pId)
        {
            if (_items.ContainsKey(pId))
                return (CatalogItem)_items[pId];
            return null;
        }
    }
}