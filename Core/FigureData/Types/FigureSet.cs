using System.Collections.Generic;

namespace Plus.Core.FigureData.Types
{
    class FigureSet
    {
        public SetType Type { get; set; }
        public int PalletId { get; set; }
        public Dictionary<int, Set> Sets { get; set; }

        public FigureSet(SetType type, int palletId)
        {
            Type = type;
            PalletId = palletId;

            Sets = new Dictionary<int, Set>();
        }
    }
}