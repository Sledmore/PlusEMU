using System.Collections.Generic;

namespace Plus.Core.FigureData.Types
{
    class Set
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public int ClubLevel { get; set; }
        public bool Colorable { get; set; }
        public bool Selectable { get; set; }
        public bool Preselectable { get; set; }

        private Dictionary<string, Part> _parts;

        public Set(int id, string gender, int clubLevel, bool colorable, bool selectable, bool preselectable)
        {
            this.Id = id;
            this.Gender = gender;
            this.ClubLevel = clubLevel;
            this.Colorable = colorable;
            this.Selectable = selectable;
            this.Preselectable = preselectable;

            this._parts = new Dictionary<string, Part>();
        }

        public Dictionary<string, Part> Parts
        {
            get { return this._parts; }
            set { this._parts = value; }
        }
    }
}
