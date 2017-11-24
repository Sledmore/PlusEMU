using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Rooms.Chat.Filter
{
    sealed class WordFilter
    {
        private string _word;
        private string _replacement;
        private bool _strict;
        private bool _bannable;

        public WordFilter(string Word, string Replacement, bool Strict, bool Bannable)
        {
            this._word = Word;
            this._replacement = Replacement;
            this._strict = Strict;
            this._bannable = Bannable;
        }

        public string Word
        {
            get { return this._word; }
        }

        public string Replacement
        {
            get { return this._replacement; }
        }
        public bool IsStrict
        {
            get { return this._strict; }
        }
        public bool IsBannable
        {
            get { return this._bannable; }
        }
    }
}
