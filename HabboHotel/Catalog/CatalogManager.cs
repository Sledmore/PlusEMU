using System;
using System.Data;
using System.Collections.Generic;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog.Pets;
using Plus.HabboHotel.Catalog.Vouchers;
using Plus.HabboHotel.Catalog.Marketplace;
using Plus.HabboHotel.Catalog.Clothing;

using Plus.Database.Interfaces;
using Serilog;

namespace Plus.HabboHotel.Catalog
{
    public class CatalogManager
    {
        private MarketplaceManager _marketplace;
        private PetRaceManager _petRaceManager;
        private VoucherManager _voucherManager;
        private ClothingManager _clothingManager;

        private Dictionary<int, int> _itemOffers;
        private readonly Dictionary<int, CatalogPage> _pages;
        private readonly Dictionary<int, CatalogBot> _botPresets;
        private readonly Dictionary<int, Dictionary<int, CatalogItem>> _items;
        private readonly Dictionary<int, CatalogDeal> _deals;
        private readonly Dictionary<int, CatalogPromotion> _promotions;

        public CatalogManager()
        {
            _marketplace = new MarketplaceManager();
            _petRaceManager = new PetRaceManager();

            _voucherManager = new VoucherManager();
            _voucherManager.Init();

            _clothingManager = new ClothingManager();
            _clothingManager.Init();

            _itemOffers = new Dictionary<int, int>();
            _pages = new Dictionary<int, CatalogPage>();
            _botPresets = new Dictionary<int, CatalogBot>();
            _items = new Dictionary<int, Dictionary<int, CatalogItem>>();
            _deals = new Dictionary<int, CatalogDeal>();
            _promotions = new Dictionary<int, CatalogPromotion>();
        }

        public void Init(ItemDataManager ItemDataManager)
        {
            if (_pages.Count > 0)
                _pages.Clear();
            if (_botPresets.Count > 0)
                _botPresets.Clear();
            if (_items.Count > 0)
                _items.Clear();
            if (_deals.Count > 0)
                _deals.Clear();
            if (_promotions.Count > 0)
                _promotions.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`item_id`,`catalog_name`,`cost_credits`,`cost_pixels`,`cost_diamonds`,`amount`,`page_id`,`limited_sells`,`limited_stack`,`offer_active`,`extradata`,`badge`,`offer_id` FROM `catalog_items`");
                DataTable CatalogueItems = dbClient.GetTable();

                if (CatalogueItems != null)
                {
                    foreach (DataRow Row in CatalogueItems.Rows)
                    {
                        if (Convert.ToInt32(Row["amount"]) <= 0)
                            continue;

                        int ItemId = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        int BaseId = Convert.ToInt32(Row["item_id"]);
                        int OfferId = Convert.ToInt32(Row["offer_id"]);
                        
                        if (!ItemDataManager.GetItem(BaseId, out ItemData Data))
                        {
                            Log.Error("Couldn't load Catalog Item " + ItemId + ", no furniture record found.");
                            continue;
                        }

                        if (!_items.ContainsKey(PageId))
                            _items[PageId] = new Dictionary<int, CatalogItem>();

                        if (OfferId != -1 && !_itemOffers.ContainsKey(OfferId))
                            _itemOffers.Add(OfferId, PageId);

                        _items[PageId].Add(Convert.ToInt32(Row["id"]), new CatalogItem(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
                            Data, Convert.ToString(Row["catalog_name"]), Convert.ToInt32(Row["page_id"]), Convert.ToInt32(Row["cost_credits"]), Convert.ToInt32(Row["cost_pixels"]), Convert.ToInt32(Row["cost_diamonds"]),
                            Convert.ToInt32(Row["amount"]), Convert.ToInt32(Row["limited_sells"]), Convert.ToInt32(Row["limited_stack"]), PlusEnvironment.EnumToBool(Row["offer_active"].ToString()),
                            Convert.ToString(Row["extradata"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["offer_id"])));
                    }
                }

                dbClient.SetQuery("SELECT `id`, `items`, `name`, `room_id` FROM `catalog_deals`");
                DataTable GetDeals = dbClient.GetTable();

                if (GetDeals != null)
                {
                    foreach (DataRow Row in GetDeals.Rows)
                    {
                        int Id = Convert.ToInt32(Row["id"]);
                        string Items = Convert.ToString(Row["items"]);
                        string Name = Convert.ToString(Row["name"]);
                        int RoomId = Convert.ToInt32(Row["room_id"]);

                        CatalogDeal Deal = new CatalogDeal(Id, Items, Name, RoomId, ItemDataManager);

                        if (!_deals.ContainsKey(Id))
                            _deals.Add(Deal.Id, Deal);
                    }
                }


                dbClient.SetQuery("SELECT `id`,`parent_id`,`caption`,`page_link`,`visible`,`enabled`,`min_rank`,`min_vip`,`icon_image`,`page_layout`,`page_strings_1`,`page_strings_2` FROM `catalog_pages` ORDER BY `order_num`");
                DataTable CatalogPages = dbClient.GetTable();

                if (CatalogPages != null)
                {
                    foreach (DataRow Row in CatalogPages.Rows)
                    {
                        _pages.Add(Convert.ToInt32(Row["id"]), new CatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToInt32(Row["min_vip"]), Row["visible"].ToString(), Convert.ToString(Row["page_layout"]), 
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]),
                            _items.ContainsKey(Convert.ToInt32(Row["id"])) ? _items[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogItem>(), ref _itemOffers));
                    }
                }

                dbClient.SetQuery("SELECT `id`,`name`,`figure`,`motto`,`gender`,`ai_type` FROM `catalog_bot_presets`");
                DataTable bots = dbClient.GetTable();

                if (bots != null)
                {
                    foreach (DataRow row in bots.Rows)
                    {
                        _botPresets.Add(Convert.ToInt32(row[0]), new CatalogBot(Convert.ToInt32(row[0]), Convert.ToString(row[1]), Convert.ToString(row[2]), Convert.ToString(row[3]), Convert.ToString(row[4]), Convert.ToString(row[5])));
                    }
                }

                dbClient.SetQuery("SELECT * FROM `catalog_promotions`");
                DataTable GetPromotions = dbClient.GetTable();

                if (GetPromotions != null)
                {
                    foreach (DataRow row in GetPromotions.Rows)
                    {
                        if (!_promotions.ContainsKey(Convert.ToInt32(row["id"])))
                            _promotions.Add(Convert.ToInt32(row["id"]), new CatalogPromotion(Convert.ToInt32(row["id"]), Convert.ToString(row["title"]), Convert.ToString(row["image"]), Convert.ToInt32(row["unknown"]), Convert.ToString(row["page_link"]), Convert.ToInt32(row["parent_id"])));
                    }
                }

                _petRaceManager.Init();
                _clothingManager.Init();
            }

            Log.Information("Catalog Manager -> LOADED");
        }

        public bool TryGetBot(int ItemId, out CatalogBot Bot)
        {
            return _botPresets.TryGetValue(ItemId, out Bot);
        }

        public Dictionary<int, int> ItemOffers
        {
            get { return _itemOffers; }
        }

        public bool TryGetPage(int pageId, out CatalogPage page)
        {
            return _pages.TryGetValue(pageId, out page);
        }

        public bool TryGetDeal(int dealId, out CatalogDeal deal)
        {
            return _deals.TryGetValue(dealId, out deal);
        }

        public ICollection<CatalogPage> GetPages()
        {
            return _pages.Values;
        }

        public ICollection<CatalogPromotion> GetPromotions()
        {
            return _promotions.Values;
        }

        public MarketplaceManager GetMarketplace()
        {
            return _marketplace;
        }

        public PetRaceManager GetPetRaceManager()
        {
            return _petRaceManager;
        }

        public VoucherManager GetVoucherManager()
        {
            return _voucherManager;
        }

        public ClothingManager GetClothingManager()
        {
            return _clothingManager;
        }
    }
}