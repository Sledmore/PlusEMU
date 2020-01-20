using Plus.Database.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;

namespace Plus.HabboHotel.Rooms.Chat.Styles
{
    public sealed class ChatStyleManager
    {
        private readonly Dictionary<int, ChatStyle> _styles;

        public ChatStyleManager()
        {
            _styles = new Dictionary<int, ChatStyle>();
        }

        public void Init()
        {
            if (_styles.Count > 0)
                _styles.Clear();

            DataTable Table = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_chat_styles`;");
                Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        try
                        {
                            if (!_styles.ContainsKey(Convert.ToInt32(Row["id"])))
                                _styles.Add(Convert.ToInt32(Row["id"]), new ChatStyle(Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["required_right"])));
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Unable to load ChatBubble for ID [" + Convert.ToInt32(Row["id"]) + "]", ex);
                        }
                    }
                }
            }

            Log.Information("Loaded " + _styles.Count + " chat styles.");
        }

        public bool TryGetStyle(int Id, out ChatStyle Style)
        {
            return _styles.TryGetValue(Id, out Style);
        }
    }
}
