namespace Plus.HabboHotel.Catalog
{
    public class CatalogPromotion
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Unknown { get; set; }
        public string PageLink { get; set; }
        public int ParentId { get; set; }

        public CatalogPromotion(int id, string title, string image, int unknown, string pageLink, int parentId)
        {
            this.Id = id;
            this.Title = title;
            this.Image = image;
            this.Unknown = unknown;
            this.PageLink = pageLink;
            this.ParentId = parentId;
        }
    }
}
