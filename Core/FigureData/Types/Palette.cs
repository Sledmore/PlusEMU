using System.Collections.Generic;

namespace Plus.Core.FigureData.Types
{
    public class Palette
    {
        public int Id { get; set; }

        private Dictionary<int, Color> _colors;

        public Palette(int id)
        {
            this.Id = id;
            this._colors = new Dictionary<int, Color>();
        }

        public Dictionary<int, Color> Colors
        {
            get { return this._colors; }
            set { this._colors = value; }
        }
    }
}
