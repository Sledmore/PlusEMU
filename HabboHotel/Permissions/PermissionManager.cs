using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using Plus.Database.Interfaces;
using System.Data;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Permissions
{
    public sealed class PermissionManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Permissions.PermissionManager");

        private readonly Dictionary<int, Permission> Permissions = new Dictionary<int, Permission>();

        private readonly Dictionary<string, PermissionCommand> _commands = new Dictionary<string, PermissionCommand>();

        private readonly Dictionary<int, PermissionGroup> PermissionGroups = new Dictionary<int, PermissionGroup>();

        private readonly Dictionary<int, List<string>> PermissionGroupRights = new Dictionary<int, List<string>>();

        private readonly Dictionary<int, List<string>> PermissionSubscriptionRights = new Dictionary<int, List<string>>();

        public PermissionManager()
        {

        }

        public void Init()
        {
            this.Permissions.Clear();
            this._commands.Clear();
            this.PermissionGroups.Clear();
            this.PermissionGroupRights.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions`");
                DataTable GetPermissions = dbClient.GetTable();

                if (GetPermissions != null)
                {
                    foreach (DataRow Row in GetPermissions.Rows)
                    {
                        this.Permissions.Add(Convert.ToInt32(Row["id"]), new Permission(Convert.ToInt32(Row["id"]), Convert.ToString(Row["permission"]), Convert.ToString(Row["description"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_commands`");
                DataTable GetCommands = dbClient.GetTable();

                if (GetCommands != null)
                {
                    foreach (DataRow Row in GetCommands.Rows)
                    {
                         this._commands.Add(Convert.ToString(Row["command"]), new PermissionCommand(Convert.ToString(Row["command"]), Convert.ToInt32(Row["group_id"]), Convert.ToInt32(Row["subscription_id"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_groups`");
                DataTable GetPermissionGroups = dbClient.GetTable();

                if (GetPermissionGroups != null)
                {
                    foreach (DataRow Row in GetPermissionGroups.Rows)
                    {
                        this.PermissionGroups.Add(Convert.ToInt32(Row["id"]), new PermissionGroup(Convert.ToString("name"), Convert.ToString("description"), Convert.ToString("badge")));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_rights`");
                DataTable GetPermissionRights = dbClient.GetTable();

                if (GetPermissionRights != null)
                {
                    foreach (DataRow Row in GetPermissionRights.Rows)
                    {
                        int GroupId = Convert.ToInt32(Row["group_id"]);
                        int PermissionId = Convert.ToInt32(Row["permission_id"]);

                        if (!this.PermissionGroups.ContainsKey(GroupId))
                        {
                            continue; // permission group does not exist
                        }

                        Permission Permission = null;

                        if (!this.Permissions.TryGetValue(PermissionId, out Permission))
                        {
                            continue; // permission does not exist
                        }

                        if (PermissionGroupRights.ContainsKey(GroupId))
                        {
                            this.PermissionGroupRights[GroupId].Add(Permission.PermissionName);
                        }
                        else
                        {
                            List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                            this.PermissionGroupRights.Add(GroupId, RightsSet);
                        }

                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_subscriptions`");
                DataTable GetPermissionSubscriptions = dbClient.GetTable();

                if (GetPermissionSubscriptions != null)
                {
                    foreach (DataRow Row in GetPermissionSubscriptions.Rows)
                    {
                        int PermissionId = Convert.ToInt32(Row["permission_id"]);
                        int SubscriptionId = Convert.ToInt32(Row["subscription_id"]);

                        Permission Permission = null;
                        if (!this.Permissions.TryGetValue(PermissionId, out Permission))
                            continue; // permission does not exist

                        if (this.PermissionSubscriptionRights.ContainsKey(SubscriptionId))
                        {
                            this.PermissionSubscriptionRights[SubscriptionId].Add(Permission.PermissionName);
                        }
                        else
                        {
                            List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                            this.PermissionSubscriptionRights.Add(SubscriptionId, RightsSet);
                        }
                    }
                }
            }

            log.Info("Loaded " + this.Permissions.Count + " permissions.");
            log.Info("Loaded " + this.PermissionGroups.Count + " permissions groups.");
            log.Info("Loaded " + this.PermissionGroupRights.Count + " permissions group rights.");
            log.Info("Loaded " + this.PermissionSubscriptionRights.Count + " permissions subscription rights.");
        }

        public bool TryGetGroup(int Id, out PermissionGroup Group)
        {
            return this.PermissionGroups.TryGetValue(Id, out Group);
        }

        public List<string> GetPermissionsForPlayer(Habbo Player)
        {
            List<string> PermissionSet = new List<string>();

            List<string> PermRights = null;
            if (this.PermissionGroupRights.TryGetValue(Player.Rank, out PermRights))
            {
                PermissionSet.AddRange(PermRights);
            }

            List<string> SubscriptionRights = null;
            if (this.PermissionSubscriptionRights.TryGetValue(Player.VIPRank, out SubscriptionRights))
            {
                PermissionSet.AddRange(SubscriptionRights);
            }

            return PermissionSet;
        }

        public List<string> GetCommandsForPlayer(Habbo Player)
        {
            return this._commands.Where(x => Player.Rank >= x.Value.GroupId && Player.VIPRank >= x.Value.SubscriptionId).Select(x => x.Key).ToList();
        }
    }
}