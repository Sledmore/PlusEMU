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
            _word = word;
            _replacement = replacement;
            _strict = strict;
            _bannable = bannable;
        }

        public string Word
        {
            get { return _word; }
        }

        public string Replacement
        {
            get { return _replacement; }
        }

        public bool IsStrict
        {
            get { return _strict; }
        }

        public bool IsBannable
        {
            get { return _bannable; }
        }
    }
}
