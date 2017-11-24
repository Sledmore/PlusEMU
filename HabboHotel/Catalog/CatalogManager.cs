using System;
using System.Data;
using System.Collections.Generic;

using log4net;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog.Pets;
using Plus.HabboHotel.Catalog.Vouchers;
using Plus.HabboHotel.Catalog.Marketplace;
using Plus.HabboHotel.Catalog.Clothing;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Catalog
{
    public class CatalogManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Catalog.CatalogManager");

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
            this._marketplace = new MarketplaceManager();
            this._petRaceManager = new PetRaceManager();

            this._voucherManager = new VoucherManager();
            this._voucherManager.Init();

            this._clothingManager = new ClothingManager();
            this._clothingManager.Init();

            this._itemOffers = new Dictionary<int, int>();
            this._pages = new Dictionary<int, CatalogPage>();
            this._botPresets = new Dictionary<int, CatalogBot>();
            this._items = new Dictionary<int, Dictionary<int, CatalogItem>>();
            this._deals = new Dictionary<int, CatalogDeal>();
            this._promotions = new Dictionary<int, CatalogPromotion>();
        }

        public void Init(ItemDataManager ItemDataManager)
        {
            if (this._pages.Count > 0)
                this._pages.Clear();
            if (this._botPresets.Count > 0)
                this._botPresets.Clear();
            if (this._items.Count > 0)
                this._items.Clear();
            if (this._deals.Count > 0)
                this._deals.Clear();
            if (this._promotions.Count > 0)
                this._promotions.Clear();

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

                        ItemData Data = null;
                        if (!ItemDataManager.GetItem(BaseId, out Data))
                        {
                            log.Error("Couldn't load Catalog Item " + ItemId + ", no furniture record found.");
                            continue;
                        }

                        if (!this._items.ContainsKey(PageId))
                            this._items[PageId] = new Dictionary<int, CatalogItem>();

                        if (OfferId != -1 && !this._itemOffers.ContainsKey(OfferId))
                            this._itemOffers.Add(OfferId, PageId);

                        this._items[PageId].Add(Convert.ToInt32(Row["id"]), new CatalogItem(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
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

                        if (!this._deals.ContainsKey(Id))
                            this._deals.Add(Deal.Id, Deal);
                    }
                }


                dbClient.SetQuery("SELECT `id`,`parent_id`,`caption`,`page_link`,`visible`,`enabled`,`min_rank`,`min_vip`,`icon_image`,`page_layout`,`page_strings_1`,`page_strings_2` FROM `catalog_pages` ORDER BY `order_num`");
                DataTable CatalogPages = dbClient.GetTable();

                if (CatalogPages != null)
                {
                    foreach (DataRow Row in CatalogPages.Rows)
                    {
                        this._pages.Add(Convert.ToInt32(Row["id"]), new CatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToInt32(Row["min_vip"]), Row["visible"].ToString(), Convert.ToString(Row["page_layout"]), 
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]),
                            this._items.ContainsKey(Convert.ToInt32(Row["id"])) ? this._items[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogItem>(), ref this._itemOffers));
                    }
                }

                dbClient.SetQuery("SELECT `id`,`name`,`figure`,`motto`,`gender`,`ai_type` FROM `catalog_bot_presets`");
                DataTable bots = dbClient.GetTable();

                if (bots != null)
                {
                    foreach (DataRow Row in bots.Rows)
                    {
                        this._botPresets.Add(Convert.ToInt32(Row[0]), new CatalogBot(Convert.ToInt32(Row[0]), Convert.ToString(Row[1]), Convert.ToString(Row[2]), Convert.ToString(Row[3]), Convert.ToString(Row[4]), Convert.ToString(Row[5])));
                    }
                }

                dbClient.SetQuery("SELECT * FROM `catalog_promotions`");
                DataTable GetPromotions = dbClient.GetTable();

                if (GetPromotions != null)
                {
                    foreach (DataRow Row in GetPromotions.Rows)
                    {
                        if (!this._promotions.ContainsKey(Convert.ToInt32(Row["id"])))
                            this._promotions.Add(Convert.ToInt32(Row["id"]), new CatalogPromotion(Convert.ToInt32(Row["id"]), Convert.ToString(Row["title"]), Convert.ToString(Row["image"]), Convert.ToInt32(Row["unknown"]), Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["parent_id"])));
                    }
                }

                this._petRaceManager.Init();
                this._clothingManager.Init();
            }

            log.Info("Catalog Manager -> LOADED");
        }

        public bool TryGetBot(int ItemId, out CatalogBot Bot)
        {
            return this._botPresets.TryGetValue(ItemId, out Bot);
        }

        public Dictionary<int, int> ItemOffers
        {
            get { return this._itemOffers; }
        }

        public bool TryGetPage(int pageId, out CatalogPage page)
        {
            return this._pages.TryGetValue(pageId, out page);
        }

        public bool TryGetDeal(int dealId, out CatalogDeal deal)
        {
            return this._deals.TryGetValue(dealId, out deal);
        }

        public ICollection<CatalogPage> GetPages()
        {
            return this._pages.Values;
        }

        public ICollection<CatalogPromotion> GetPromotions()
        {
            return this._promotions.Values;
        }

        public MarketplaceManager GetMarketplace()
        {
            return this._marketplace;
        }

        public PetRaceManager GetPetRaceManager()
        {
            return this._petRaceManager;
        }

        public VoucherManager GetVoucherManager()
        {
            return this._voucherManager;
        }

        public ClothingManager GetClothingManager()
        {
            return this._clothingManager;
        }
    }
}