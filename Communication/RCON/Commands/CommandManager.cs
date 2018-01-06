using Plus.Communication.Rcon.Commands.Hotel;
using Plus.Communication.Rcon.Commands.User;
using System;
using System.Collections.Generic;

namespace Plus.Communication.Rcon.Commands
{
    public class CommandManager
    { 
        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IRconCommand> _commands;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager()
        {
            _commands = new Dictionary<string, IRconCommand>();
            
            RegisterUser();
            RegisterHotel();
        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="data">A string of data split by char(1), the first part being the command and the second part being the parameters.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(string data)
        {
            if (data.Length == 0 || string.IsNullOrEmpty(data))
                return false;

            string cmd = data.Split(Convert.ToChar(1))[0];

            if (_commands.TryGetValue(cmd.ToLower(), out IRconCommand command))
            {
                string[] parameters = null;
                if (data.Split(Convert.ToChar(1))[1] != null)
                {
                    var param = data.Split(Convert.ToChar(1))[1];
                    parameters = param.Split(':');
                }

                return command.TryExecute(parameters);
            }
            return false;
        }

        /// <summary>
        /// Registers the commands tailored towards a user.
        /// </summary>
        private void RegisterUser()
        {
            Register("alert_user", new AlertUserCommand());
            Register("disconnect_user", new DisconnectUserCommand());
            Register("reload_user_motto", new ReloadUserMottoCommand());
            Register("give_user_currency", new GiveUserCurrencyCommand());
            Register("take_user_currency", new TakeUserCurrencyCommand());
            Register("sync_user_currency", new SyncUserCurrencyCommand());
            Register("reload_user_currency", new ReloadUserCurrencyCommand());
            Register("reload_user_rank", new ReloadUserRankCommand());
            Register("reload_user_vip_rank", new ReloadUserVIPRankCommand());
            Register("progress_user_achievement", new ProgressUserAchievementCommand());
            Register("give_user_badge", new GiveUserBadgeCommand());
            Register("take_user_badge", new TakeUserBadgeCommand());
        }   

        /// <summary>
        /// Registers the commands tailored towards the hotel.
        /// </summary>
        private void RegisterHotel()
        {
            Register("reload_bans", new ReloadBansCommand());
            Register("reload_quests", new ReloadQuestsCommand());
            Register("reload_server_settings", new ReloadServerSettingsCommand());
            Register("reload_vouchers", new ReloadVouchersCommand());
            Register("reload_ranks", new ReloadRanksCommand());
            Register("reload_navigator", new ReloadNavigatorCommand());
            Register("reload_items", new ReloadItemsCommand());
            Register("reload_catalog", new ReloadCatalogCommand());
            Register("reload_filter", new ReloadFilterCommand());
        }

        /// <summary>
        /// Registers a Rcon command.
        /// </summary>
        /// <param name="commandText">Text to type for this command.</param>
        /// <param name="command">The command to execute.</param>
        public void Register(string commandText, IRconCommand command)
        {
            _commands.Add(commandText, command);
        }
    }
}