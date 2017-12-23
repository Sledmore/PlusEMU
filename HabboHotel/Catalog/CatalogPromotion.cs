namespace Plus.HabboHotel.Catalog
{
    public class CatalogPromotion
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Image { get; private set; }
        public int Unknown { get; private set; }
        public string PageLink { get; private set; }
        public int ParentId { get; private set; }

        public CatalogPromotion(int id, string title, string image, int unknown, string pageLink, int parentId)
        {
            Id = id;
            Title = title;
            Image = image;
            Unknown = unknown;
            PageLink = pageLink;
            ParentId = parentId;
        }
    }
}
