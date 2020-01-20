﻿using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.AI.Responses;
using Plus.HabboHotel.Rooms.AI;

namespace Plus.HabboHotel.Bots
{
    public class BotManager
    {
        private readonly List<BotResponse> _responses;

        public BotManager()
        {
            _responses = new List<BotResponse>();
        }

        public void Init()
        {
            if (_responses.Count > 0)
                _responses.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `bot_ai`,`chat_keywords`,`response_text`,`response_mode`,`response_beverage` FROM `bots_responses`");
                DataTable data = dbClient.GetTable();

                if (data != null)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        _responses.Add(new BotResponse(Convert.ToString(row["bot_ai"]), Convert.ToString(row["chat_keywords"]), Convert.ToString(row["response_text"]), row["response_mode"].ToString(), Convert.ToString(row["response_beverage"])));
                    }
                }
            }
        }

        public BotResponse GetResponse(BotAIType type, string message)
        {
            foreach (BotResponse response in _responses.Where(x => x.AiType == type).ToList())
            {
                if (response.KeywordMatched(message))
                {
                    return response;
                }
            }

            return null;
        }
    }
}
