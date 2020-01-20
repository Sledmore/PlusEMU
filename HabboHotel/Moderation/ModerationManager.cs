using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Plus.Database.Interfaces;
using Serilog;

namespace Plus.HabboHotel.Moderation
{
    public sealed class ModerationManager
    {
        private int _ticketCount = 1;
        private List<string> _userPresets = new List<string>();
        private List<string> _roomPresets = new List<string>();
        private Dictionary<string, ModerationBan> _bans = new Dictionary<string, ModerationBan>();
        private Dictionary<int, string> _userActionPresetCategories = new Dictionary<int, string>();
        private Dictionary<int, List<ModerationPresetActionMessages>> _userActionPresetMessages = new Dictionary<int, List<ModerationPresetActionMessages>>();
        private ConcurrentDictionary<int, ModerationTicket> _modTickets = new ConcurrentDictionary<int, ModerationTicket>();


        private Dictionary<int, string> _moderationCFHTopics = new Dictionary<int, string>();
        private Dictionary<int, List<ModerationPresetActions>> _moderationCFHTopicActions = new Dictionary<int, List<ModerationPresetActions>>();

        public ModerationManager()
        {

        }

        public void Init()
        {
            if (_userPresets.Count > 0)
                _userPresets.Clear();
            if (_moderationCFHTopics.Count > 0)
                _moderationCFHTopics.Clear();
            if (_moderationCFHTopicActions.Count > 0)
                _moderationCFHTopicActions.Clear();
            if (_bans.Count > 0)
                _bans.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable PresetsTable = null;
                dbClient.SetQuery("SELECT * FROM `moderation_presets`;");
                PresetsTable = dbClient.GetTable();

                if (PresetsTable != null)
                {
                    foreach (DataRow Row in PresetsTable.Rows)
                    {
                        string Type = Convert.ToString(Row["type"]).ToLower();
                        switch (Type)
                        {
                            case "user":
                                _userPresets.Add(Convert.ToString(Row["message"]));
                                break;

                            case "room":
                                _roomPresets.Add(Convert.ToString(Row["message"]));
                                break;
                        }
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable ModerationTopics = null;
                dbClient.SetQuery("SELECT * FROM `moderation_topics`;");
                ModerationTopics = dbClient.GetTable();

                if (ModerationTopics != null)
                {
                    foreach (DataRow Row in ModerationTopics.Rows)
                    {
                        if (!_moderationCFHTopics.ContainsKey(Convert.ToInt32(Row["id"])))
                            _moderationCFHTopics.Add(Convert.ToInt32(Row["id"]), Convert.ToString(Row["caption"]));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable ModerationTopicsActions = null;
                dbClient.SetQuery("SELECT * FROM `moderation_topic_actions`;");
                ModerationTopicsActions = dbClient.GetTable();

                if (ModerationTopicsActions != null)
                {
                    foreach (DataRow Row in ModerationTopicsActions.Rows)
                    {
                        int ParentId = Convert.ToInt32(Row["parent_id"]);

                        if (!_moderationCFHTopicActions.ContainsKey(ParentId))
                        {
                            _moderationCFHTopicActions.Add(ParentId, new List<ModerationPresetActions>());
                        }

                        _moderationCFHTopicActions[ParentId].Add(new ModerationPresetActions(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Convert.ToString(Row["type"]), Convert.ToString(Row["caption"]), Convert.ToString(Row["message_text"]),
                            Convert.ToInt32(Row["mute_time"]), Convert.ToInt32(Row["ban_time"]), Convert.ToInt32(Row["ip_time"]), Convert.ToInt32(Row["trade_lock_time"]), Convert.ToString(Row["default_sanction"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable PresetsActionCats = null;
                dbClient.SetQuery("SELECT * FROM `moderation_preset_action_categories`;");
                PresetsActionCats = dbClient.GetTable();

                if (PresetsActionCats != null)
                {
                    foreach (DataRow Row in PresetsActionCats.Rows)
                    {
                        _userActionPresetCategories.Add(Convert.ToInt32(Row["id"]), Convert.ToString(Row["caption"]));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable PresetsActionMessages = null;
                dbClient.SetQuery("SELECT * FROM `moderation_preset_action_messages`;");
                PresetsActionMessages = dbClient.GetTable();

                if (PresetsActionMessages != null)
                {
                    foreach (DataRow Row in PresetsActionMessages.Rows)
                    {
                        int ParentId = Convert.ToInt32(Row["parent_id"]);

                        if (!_userActionPresetMessages.ContainsKey(ParentId))
                        {
                            _userActionPresetMessages.Add(ParentId, new List<ModerationPresetActionMessages>());
                        }

                        _userActionPresetMessages[ParentId].Add(new ModerationPresetActionMessages(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Convert.ToString(Row["caption"]), Convert.ToString(Row["message_text"]),
                            Convert.ToInt32(Row["mute_hours"]), Convert.ToInt32(Row["ban_hours"]), Convert.ToInt32(Row["ip_ban_hours"]), Convert.ToInt32(Row["trade_lock_days"]), Convert.ToString(Row["notice"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetBans = null;
                dbClient.SetQuery("SELECT `bantype`,`value`,`reason`,`expire` FROM `bans` WHERE `bantype` = 'machine' OR `bantype` = 'user'");
                GetBans = dbClient.GetTable();

                if (GetBans != null)
                {
                    foreach (DataRow dRow in GetBans.Rows)
                    {
                        string value = Convert.ToString(dRow["value"]);
                        string reason = Convert.ToString(dRow["reason"]);
                        double expires = (double)dRow["expire"];
                        string type = Convert.ToString(dRow["bantype"]);

                        ModerationBan Ban = new ModerationBan(BanTypeUtility.GetModerationBanType(type), value, reason, expires);
                        if (Ban != null)
                        {
                            if (expires > PlusEnvironment.GetUnixTimestamp())
                            {
                                if (!_bans.ContainsKey(value))
                                    _bans.Add(value, Ban);
                            }
                            else
                            {
                                dbClient.SetQuery("DELETE FROM `bans` WHERE `bantype` = '" + BanTypeUtility.FromModerationBanType(Ban.Type) + "' AND `value` = @Key LIMIT 1");
                                dbClient.AddParameter("Key", value);
                                dbClient.RunQuery();
                            }
                        }
                    }
                }
            }

            Log.Information("Loaded " + (_userPresets.Count + _roomPresets.Count) + " moderation presets.");
            Log.Information("Loaded " + _userActionPresetCategories.Count + " moderation categories.");
            Log.Information("Loaded " + _userActionPresetMessages.Count + " moderation action preset messages.");
            Log.Information("Cached " + _bans.Count + " username and machine bans.");
        }

        public void ReCacheBans()
        {
            if (_bans.Count > 0)
                _bans.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetBans = null;
                dbClient.SetQuery("SELECT `bantype`,`value`,`reason`,`expire` FROM `bans` WHERE `bantype` = 'machine' OR `bantype` = 'user'");
                GetBans = dbClient.GetTable();

                if (GetBans != null)
                {
                    foreach (DataRow dRow in GetBans.Rows)
                    {
                        string value = Convert.ToString(dRow["value"]);
                        string reason = Convert.ToString(dRow["reason"]);
                        double expires = (double)dRow["expire"];
                        string type = Convert.ToString(dRow["bantype"]);

                        ModerationBan Ban = new ModerationBan(BanTypeUtility.GetModerationBanType(type), value, reason, expires);
                        if (Ban != null)
                        {
                            if (expires > PlusEnvironment.GetUnixTimestamp())
                            {
                                if (!_bans.ContainsKey(value))
                                    _bans.Add(value, Ban);
                            }
                            else
                            {
                                dbClient.SetQuery("DELETE FROM `bans` WHERE `bantype` = '" + BanTypeUtility.FromModerationBanType(Ban.Type) + "' AND `value` = @Key LIMIT 1");
                                dbClient.AddParameter("Key", value);
                                dbClient.RunQuery();
                            }
                        }
                    }
                }
            }

            Log.Information("Cached " + _bans.Count + " username and machine bans.");
        }

        public void BanUser(string Mod, ModerationBanType Type, string BanValue, string Reason, double ExpireTimestamp)
        {
            string BanType = (Type == ModerationBanType.IP ? "ip" : Type == ModerationBanType.Machine ? "machine" : "user");
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO `bans` (`bantype`, `value`, `reason`, `expire`, `added_by`,`added_date`) VALUES ('" + BanType + "', '" + BanValue + "', @reason, " + ExpireTimestamp + ", '" + Mod + "', '" + PlusEnvironment.GetUnixTimestamp() + "');");
                dbClient.AddParameter("reason", Reason);
                dbClient.RunQuery();
            }

            if (Type == ModerationBanType.Machine || Type == ModerationBanType.Username)
            {
                if (!_bans.ContainsKey(BanValue))
                    _bans.Add(BanValue, new ModerationBan(Type, BanValue, Reason, ExpireTimestamp));
            }
        }

        public ICollection<string> UserMessagePresets
        {
            get { return _userPresets; }
        }

        public ICollection<string> RoomMessagePresets
        {
            get { return _roomPresets; }
        }

        public ICollection<ModerationTicket> GetTickets
        {
            get { return _modTickets.Values; }
        }

        public Dictionary<string, List<ModerationPresetActions>> UserActionPresets
        {
            get
            {
                Dictionary<string, List<ModerationPresetActions>> Result = new Dictionary<string, List<ModerationPresetActions>>();
                foreach (KeyValuePair<int, string> Category in _moderationCFHTopics.ToList())
                {
                    Result.Add(Category.Value, new List<ModerationPresetActions>());

                    if (_moderationCFHTopicActions.ContainsKey(Category.Key))
                    {
                        foreach (ModerationPresetActions Data in _moderationCFHTopicActions[Category.Key])
                        {
                            Result[Category.Value].Add(Data);
                        }
                    }
                }
                return Result;
            }
        }

        public bool TryAddTicket(ModerationTicket Ticket)
        {
            Ticket.Id = _ticketCount++;
            return _modTickets.TryAdd(Ticket.Id, Ticket);
        }

        public bool TryGetTicket(int TicketId, out ModerationTicket Ticket)
        {
            return _modTickets.TryGetValue(TicketId, out Ticket);
        }

        public bool UserHasTickets(int userId)
        {
            return _modTickets.Count(x => x.Value.Sender.Id == userId && x.Value.Answered == false) > 0;
        }

        public ModerationTicket GetTicketBySenderId(int userId)
        {
            return _modTickets.FirstOrDefault(x => x.Value.Sender.Id == userId).Value;
        }

        /// <summary>
        /// Runs a quick check to see if a ban record is cached in the server.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Ban"></param>
        /// <returns></returns>
        public bool IsBanned(string Key, out ModerationBan Ban)
        {
            if (_bans.TryGetValue(Key, out Ban))
            {
                if (!Ban.Expired)
                    return true;

                //This ban has expired, let us quickly remove it here.
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `bans` WHERE `bantype` = '" + BanTypeUtility.FromModerationBanType(Ban.Type) + "' AND `value` = @Key LIMIT 1");
                    dbClient.AddParameter("Key", Key);
                    dbClient.RunQuery();
                }

                //And finally, let us remove the ban record from the cache.
                if (_bans.ContainsKey(Key))
                    _bans.Remove(Key);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Run a quick database check to see if this ban exists in the database.
        /// </summary>
        /// <param name="MachineId">The value of the ban.</param>
        /// <returns></returns>
        public bool MachineBanCheck(string MachineId)
        {
            ModerationBan MachineBanRecord = null;
            if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(MachineId, out MachineBanRecord))
            {
                DataRow BanRow = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `bans` WHERE `bantype` = 'machine' AND `value` = @value LIMIT 1");
                    dbClient.AddParameter("value", MachineId);
                    BanRow = dbClient.GetRow();

                    //If there is no more ban record, then we can simply remove it from our cache!
                    if (BanRow == null)
                    {
                        PlusEnvironment.GetGame().GetModerationManager().RemoveBan(MachineId);
                        return false;
                    }
                }
            }
            return true;
        }
       
        /// <summary>
        /// Run a quick database check to see if this ban exists in the database.
        /// </summary>
        /// <param name="Username">The value of the ban.</param>
        /// <returns></returns>
        public bool UsernameBanCheck(string Username)
        {
            ModerationBan UsernameBanRecord = null;
            if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(Username, out UsernameBanRecord))
            {
                DataRow BanRow = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `bans` WHERE `bantype` = 'user' AND `value` = @value LIMIT 1");
                    dbClient.AddParameter("value", Username);
                    BanRow = dbClient.GetRow();

                    //If there is no more ban record, then we can simply remove it from our cache!
                    if (BanRow == null)
                    {
                        PlusEnvironment.GetGame().GetModerationManager().RemoveBan(Username);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Remove a ban from the cache based on a given value.
        /// </summary>
        /// <param name="Value"></param>
        public void RemoveBan(string Value)
        {
            _bans.Remove(Value);
        }
    }
}
