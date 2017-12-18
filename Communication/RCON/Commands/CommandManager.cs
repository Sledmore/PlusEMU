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
            this._commands = new Dictionary<string, IRconCommand>();
            
            this.RegisterUser();
            this.RegisterHotel();
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

            IRconCommand command = null;
            if (this._commands.TryGetValue(cmd.ToLower(), out command))
            {
                string param = null;
                string[] parameters = null;
                if (data.Split(Convert.ToChar(1))[1] != null)
                {
                    param = data.Split(Convert.ToChar(1))[1];
                    parameters = param.ToString().Split(':');
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
            this.Register("alert_user", new AlertUserCommand());
            this.Register("disconnect_user", new DisconnectUserCommand());
            this.Register("reload_user_motto", new ReloadUserMottoCommand());
            this.Register("give_user_currency", new GiveUserCurrencyCommand());
            this.Register("take_user_currency", new TakeUserCurrencyCommand());
            this.Register("sync_user_currency", new SyncUserCurrencyCommand());
            this.Register("reload_user_currency", new ReloadUserCurrencyCommand());
            this.Register("reload_user_rank", new ReloadUserRankCommand());
            this.Register("reload_user_vip_rank", new ReloadUserVIPRankCommand());
            this.Register("progress_user_achievement", new ProgressUserAchievementCommand());
            this.Register("give_user_badge", new GiveUserBadgeCommand());
            this.Register("take_user_badge", new TakeUserBadgeCommand());
        }   

        /// <summary>
        /// Registers the commands tailored towards the hotel.
        /// </summary>
        private void RegisterHotel()
        {
            this.Register("reload_bans", new ReloadBansCommand());
            this.Register("reload_quests", new ReloadQuestsCommand());
            this.Register("reload_server_settings", new ReloadServerSettingsCommand());
            this.Register("reload_vouchers", new ReloadVouchersCommand());
            this.Register("reload_ranks", new ReloadRanksCommand());
            this.Register("reload_navigator", new ReloadNavigatorCommand());
            this.Register("reload_items", new ReloadItemsCommand());
            this.Register("reload_catalog", new ReloadCatalogCommand());
            this.Register("reload_filter", new ReloadFilterCommand());
        }

        /// <summary>
        /// Registers a Rcon command.
        /// </summary>
        /// <param name="commandText">Text to type for this command.</param>
        /// <param name="command">The command to execute.</param>
        public void Register(string commandText, IRconCommand command)
        {
            this._commands.Add(commandText, command);
        }
    }
}