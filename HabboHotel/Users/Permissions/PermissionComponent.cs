using System.Collections.Generic;

namespace Plus.HabboHotel.Users.Permissions
{
    /// <summary>
    /// Permissions for a specific Player.
    /// </summary>
    public sealed class PermissionComponent
    {
        /// <summary>
        /// Permission rights are stored here.
        /// </summary>
        private readonly List<string> _permissions;

        private readonly List<string> _commands;

        public PermissionComponent()
        {
            this._permissions = new List<string>();
            this._commands = new List<string>();
        }

        /// <summary>
        /// Initialize the PermissionComponent.
        /// </summary>
        /// <param name="habbo"></param>
        public bool Init(Habbo habbo)
        {
            if (this._permissions.Count > 0)
                this._permissions.Clear();

            if (this._commands.Count > 0)
                this._commands.Clear();

            this._permissions.AddRange(PlusEnvironment.GetGame().GetPermissionManager().GetPermissionsForPlayer(habbo));
            this._commands.AddRange(PlusEnvironment.GetGame().GetPermissionManager().GetCommandsForPlayer(habbo));
            return true;
        }

        /// <summary>
        /// Checks if the user has the specified right.
        /// </summary>
        /// <param name="Right"></param>
        /// <returns></returns>
        public bool HasRight(string Right)
        {
            return this._permissions.Contains(Right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public bool HasCommand(string Command)
        {
            return this._commands.Contains(Command);
        }

        /// <summary>
        /// Dispose of the permissions list.
        /// </summary>
        public void Dispose()
        {
            this._permissions.Clear();
        }
    }
}
