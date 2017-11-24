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

        public CatalogPage(int Id, int ParentId, string Enabled, string Caption, string PageLink, int Icon, int MinRank, int MinVIP,
              string Visible, string Template, string PageStrings1, string PageStrings2, Dictionary<int, CatalogItem> Items, ref Dictionary<int, int> flatOffers)
        {
            this._id = Id;
            this._parentId = ParentId;
            this._enabled = Enabled.ToLower() == "1" ? true : false;
            this._caption = Caption;
            this._pageLink = PageLink;
            this._icon = Icon;
            this._minRank = MinRank;
            this._minVIP = MinVIP;
            this._visible = Visible.ToLower() == "1" ? true : false;
            this._template = Template;

            foreach (string Str in PageStrings1.Split('|'))
            {
                if (this._pageStrings1 == null) { this._pageStrings1 = new List<string>(); }
                this._pageStrings1.Add(Str);
            }

            foreach (string Str in PageStrings2.Split('|'))
            {
                if (this._pageStrings2 == null) { this._pageStrings2 = new List<string>(); }
                this._pageStrings2.Add(Str);
            }

            this._items = Items;

            this._itemOffers = new Dictionary<int, CatalogItem>();
            foreach (int i in flatOffers.Keys)
            {
                if (flatOffers[i] == Id)
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
            get { return this._id; }
            set { this._id = value; }
        }

        public int ParentId
        {
            get { return this._parentId; }
            set { this._parentId = value; }
        }

        public bool Enabled
        {
            get { return this._enabled; }
            set { this._enabled = value; }
        }

        public string Caption
        {
            get { return this._caption; }
            set { this._caption = value; }
        }

        public string PageLink
        {
            get { return this._pageLink; }
            set { this._pageLink = value; }
        }

        public int Icon
        {
            get { return this._icon; }
            set { this._icon = value; }
        }

        public int MinimumRank
        {
            get { return this._minRank; }
            set { this._minRank = value; }
        }

        public int MinimumVIP
        {
            get { return this._minVIP;}
            set { this._minVIP = value; }
        }

        public bool Visible
        {
            get { return this._visible; }
            set { this._visible = value; }
        }

        public string Template
        {
            get { return this._template; }
            set { this._template = value; }
        }

        public List<string> PageStrings1
        {
            get { return this._pageStrings1; }
            private set { this._pageStrings1 = value; }
        }

        public List<string> PageStrings2
        {
            get { return this._pageStrings2; }
            private set { this._pageStrings2 = value; }
        }

        public Dictionary<int, CatalogItem> Items
        {
            get { return this._items; }
            private set { this._items = value; }
        }
        
        public Dictionary<int, CatalogItem> ItemOffers
        {
            get { return this._itemOffers; }
            private set { this._itemOffers = value; }
        }

        public CatalogItem GetItem(int pId)
        {
            if (this._items.ContainsKey(pId))
                return (CatalogItem)this._items[pId];
            return null;
        }
    }
}