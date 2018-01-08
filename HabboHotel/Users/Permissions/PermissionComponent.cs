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
            _permissions = new List<string>();
            _commands = new List<string>();
        }

        /// <summary>
        /// Initialize the PermissionComponent.
        /// </summary>
        /// <param name="habbo"></param>
        public bool Init(Habbo habbo)
        {
            if (_permissions.Count > 0)
                _permissions.Clear();

            if (_commands.Count > 0)
                _commands.Clear();

            _permissions.AddRange(PlusEnvironment.GetGame().GetPermissionManager().GetPermissionsForPlayer(habbo));
            _commands.AddRange(PlusEnvironment.GetGame().GetPermissionManager().GetCommandsForPlayer(habbo));
            return true;
        }

        /// <summary>
        /// Checks if the user has the specified right.
        /// </summary>
        /// <param name="Right"></param>
        /// <returns></returns>
        public bool HasRight(string Right)
        {
            return _permissions.Contains(Right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public bool HasCommand(string Command)
        {
            return _commands.Contains(Command);
        }

        /// <summary>
        /// Dispose of the permissions list.
        /// </summary>
        public void Dispose()
        {
            _permissions.Clear();
        }
    }
}
