namespace Plus.HabboHotel.Users.Clothing.Parts
{
    public sealed class ClothingParts
    {
        private int _id;
        private int _partId;
        private string _part;

        public ClothingParts(int Id, int PartId, string Part)
        {
            this._id = Id;
            this._partId = PartId;
            this._part = Part;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public int PartId
        {
            get { return this._partId; }
            set { this._partId = value; }
        }

        public string Part
        {
            get { return this._part; }
            set { this._part = value; }
        }
    }
}