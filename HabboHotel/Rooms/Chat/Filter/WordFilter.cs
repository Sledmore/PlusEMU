namespace Plus.HabboHotel.Rooms.Chat.Filter
{
    sealed class WordFilter
    {
        private string _word;
        private string _replacement;
        private bool _strict;
        private bool _bannable;

        public WordFilter(string word, string replacement, bool strict, bool bannable)
        {
            this._word = word;
            this._replacement = replacement;
            this._strict = strict;
            this._bannable = bannable;
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
