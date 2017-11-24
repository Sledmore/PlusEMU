using System.Collections.Generic;

namespace Plus.Core.FigureData.Types
{
    class FigureSet
    {
        public SetType Type { get; set; }
        public int PalletId { get; set; }

        private Dictionary<int, Set> _sets;

        public FigureSet(SetType type, int palletId)
        {
            this.Type = type;
            this.PalletId = palletId;

            this._sets = new Dictionary<int, Set>();
        }

        public Dictionary<int, Set> Sets
        {
            get { return this._sets; }
            set { this._sets = value; }
        }
    }
}