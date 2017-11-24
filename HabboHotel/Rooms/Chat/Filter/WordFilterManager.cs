using System;
using System.Linq;
using System.Text;
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

            DataTable Data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `wordfilter`");
                Data = dbClient.GetTable();

                if (Data != null)
                {
                    foreach (DataRow Row in Data.Rows)
                    {
                        this._filteredWords.Add(new WordFilter(Convert.ToString(Row["word"]), Convert.ToString(Row["replacement"]), PlusEnvironment.EnumToBool(Row["strict"].ToString()), PlusEnvironment.EnumToBool(Row["bannable"].ToString())));
                    }
                }
            }
        }

        public string CheckMessage(string Message)
        {
            foreach (WordFilter Filter in this._filteredWords.ToList())
            {
                if (Message.ToLower().Contains(Filter.Word) && Filter.IsStrict || Message == Filter.Word)
                {
                    Message = Regex.Replace(Message, Filter.Word, Filter.Replacement, RegexOptions.IgnoreCase);
                }
                else if (Message.ToLower().Contains(Filter.Word) && !Filter.IsStrict || Message == Filter.Word)
                {
                    string[] Words = Message.Split(' ');

                    Message = "";
                    foreach (string Word in Words.ToList())
                    {
                        if (Word.ToLower() == Filter.Word)
                            Message += Filter.Replacement + " ";
                        else
                            Message += Word + " ";
                    }
                }
            }

            return Message.TrimEnd(' ');
        }

        public bool CheckBannedWords(string Message)
        {
            Message = Message.Replace(" ", "").Replace(".", "").Replace("_", "").ToLower();

            foreach (WordFilter Filter in this._filteredWords.ToList())
            {
                if (!Filter.IsBannable)
                    continue;

                if (Message.Contains(Filter.Word))
                    return true;
            }
            return false;
        }

        public bool IsFiltered(string Message)
        {
            foreach (WordFilter Filter in this._filteredWords.ToList())
            {
                if (Message.Contains(Filter.Word))
                    return true;
            }
            return false;
        }
    }
}