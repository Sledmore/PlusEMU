using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

using System.Text.RegularExpressions;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms.Chat.Filter
{
    public sealed class WordFilterManager
    {
        private List<WordFilter> _filteredWords;

        public WordFilterManager()
        {
            this._filteredWords = new List<WordFilter>();
        }

        public void Init()
        {
            if (this._filteredWords.Count > 0)
                this._filteredWords.Clear();

            DataTable data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `wordfilter`");
                data = dbClient.GetTable();

                if (data != null)
                {
                    foreach (DataRow Row in data.Rows)
                    {
                        this._filteredWords.Add(new WordFilter(Convert.ToString(Row["word"]), Convert.ToString(Row["replacement"]), PlusEnvironment.EnumToBool(Row["strict"].ToString()), PlusEnvironment.EnumToBool(Row["bannable"].ToString())));
                    }
                }
            }
        }

        public string CheckMessage(string message)
        {
            foreach (WordFilter Filter in this._filteredWords.ToList())
            {
                if (message.ToLower().Contains(Filter.Word) && Filter.IsStrict || message == Filter.Word)
                {
                    message = Regex.Replace(message, Filter.Word, Filter.Replacement, RegexOptions.IgnoreCase);
                }
                else if (message.ToLower().Contains(Filter.Word) && !Filter.IsStrict || message == Filter.Word)
                {
                    string[] Words = message.Split(' ');

                    message = "";
                    foreach (string Word in Words.ToList())
                    {
                        if (Word.ToLower() == Filter.Word)
                            message += Filter.Replacement + " ";
                        else
                            message += Word + " ";
                    }
                }
            }

            return message.TrimEnd(' ');
        }

        public bool CheckBannedWords(string message)
        {
            message = message.Replace(" ", "").Replace(".", "").Replace("_", "").ToLower();

            foreach (WordFilter Filter in this._filteredWords.ToList())
            {
                if (!Filter.IsBannable)
                    continue;

                if (message.Contains(Filter.Word))
                    return true;
            }
            return false;
        }

        public bool IsFiltered(string message)
        {
            foreach (WordFilter filter in this._filteredWords.ToList())
            {
                if (message.Contains(filter.Word))
                    return true;
            }
            return false;
        }
    }
}