namespace Plus.HabboHotel.Catalog
{
    public class CatalogBot
    {
        public int Id { get; private set; }
        public string Figure { get; private set; }
        public string Gender { get; private set; }
        public string Motto { get; private set; }
        public string Name { get; private set; }
        public string AIType { get; private set; }

        public CatalogBot(int id, string name, string figure, string motto, string gender, string type)
        {
            Id = id;
            Name = name;
            Figure = figure;
            Motto = motto;
            Gender = gender;
            AIType = type;
        }
    }
}