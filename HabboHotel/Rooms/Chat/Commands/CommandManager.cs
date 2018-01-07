using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.HabboHotel.GameClients;

using Plus.HabboHotel.Rooms.Chat.Commands.User;
using Plus.HabboHotel.Rooms.Chat.Commands.User.Fun;
using Plus.HabboHotel.Rooms.Chat.Commands.Moderator;
using Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun;
using Plus.HabboHotel.Rooms.Chat.Commands.Administrator;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.Chat.Commands.Events;
using Plus.HabboHotel.Items.Wired;


namespace Plus.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        /// <summary>
        /// Command Prefix only applies to custom commands.
        /// </summary>
        private string _prefix = ":";

        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IChatCommand> _commands;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager(string Prefix)
        {
            this._prefix = Prefix;
            this._commands = new Dictionary<string, IChatCommand>();

            this.RegisterVIP();
            this.RegisterUser();
            this.RegisterEvents();
            this.RegisterModerator();
            this.RegisterAdministrator();
        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="Session">Session calling this method.</param>
        /// <param name="Message">The message to parse.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(GameClient Session, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
                return false;

            if (!Message.StartsWith(_prefix))
                return false;

            if (Message == _prefix + "commands")
            {
                StringBuilder List = new StringBuilder();
                List.Append("This is the list of commands you have available:\n");
                foreach (var CmdList in _commands.ToList())
                {
                    if (!string.IsNullOrEmpty(CmdList.Value.PermissionRequired))
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand(CmdList.Value.PermissionRequired))
                            continue;
                    }

                    List.Append(":" + CmdList.Key + " " + CmdList.Value.Parameters + " - " + CmdList.Value.Description + "\n");
                }
                Session.SendPacket(new MotdNotificationComposer(List.ToString()));
                return true;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

            IChatCommand Cmd = null;
            if (_commands.TryGetValue(Split[0].ToLower(), out Cmd))
            {
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                    this.LogCommand(Session.GetHabbo().Id, Message, Session.GetHabbo().MachineId);

                if (!string.IsNullOrEmpty(Cmd.PermissionRequired))
                {
                    if (!Session.GetHabbo().GetPermissions().HasCommand(Cmd.PermissionRequired))
                        return false;
                }


                Session.GetHabbo().IChatCommand = Cmd;
                Session.GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, Session.GetHabbo(), this);

                Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, Split);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Registers the VIP set of commands.
        /// </summary>
        private void RegisterVIP()
        {
            this.Register("spull", new SuperPullCommand());
        }

        /// <summary>
        /// Registers the Events set of commands.
        /// </summary>
        private void RegisterEvents()
        {
            this.Register("eha", new EventAlertCommand());
            this.Register("eventalert", new EventAlertCommand());
        }

        /// <summary>
        /// Registers the default set of commands.
        /// </summary>
        private void RegisterUser()
        {
            this.Register("about", new InfoCommand());
            this.Register("pickall", new PickAllCommand());
            this.Register("ejectall", new EjectAllCommand());
            this.Register("lay", new LayCommand());
            this.Register("sit", new SitCommand());
            this.Register("stand", new StandCommand());
            this.Register("mutepets", new MutePetsCommand());
            this.Register("mutebots", new MuteBotsCommand());

            this.Register("mimic", new MimicCommand());
            this.Register("dance", new DanceCommand());
            this.Register("push", new PushCommand());
            this.Register("pull", new PullCommand());
            this.Register("enable", new EnableCommand());
            this.Register("follow", new FollowCommand());
            this.Register("faceless", new FacelessCommand());
            this.Register("moonwalk", new MoonwalkCommand());

            this.Register("unload", new UnloadCommand());
            this.Register("regenmaps", new RegenMaps());
            this.Register("emptyitems", new EmptyItems());
            this.Register("setmax", new SetMaxCommand());
            this.Register("setspeed", new SetSpeedCommand());
            this.Register("disablediagonal", new DisableDiagonalCommand());
            this.Register("flagme", new FlagMeCommand());

            this.Register("stats", new StatsCommand());
            this.Register("kickpets", new KickPetsCommand());
            this.Register("kickbots", new KickBotsCommand());

            this.Register("room", new RoomCommand());
            this.Register("dnd", new DNDCommand());
            this.Register("disablegifts", new DisableGiftsCommand());
            this.Register("convertcredits", new ConvertCreditsCommand());
            this.Register("disablewhispers", new DisableWhispersCommand());
            this.Register("disablemimic", new DisableMimicCommand()); ;

            this.Register("pet", new PetCommand());
            this.Register("spush", new SuperPushCommand());
            this.Register("superpush", new SuperPushCommand());

        }

        /// <summary>
        /// Registers the moderator set of commands.
        /// </summary>
        private void RegisterModerator()
        {
            this.Register("ban", new BanCommand());
            this.Register("mip", new MIPCommand());
            this.Register("ipban", new IPBanCommand());

            this.Register("ui", new UserInfoCommand());
            this.Register("userinfo", new UserInfoCommand());
            this.Register("sa", new StaffAlertCommand());
            this.Register("roomunmute", new RoomUnmuteCommand());
            this.Register("roommute", new RoomMuteCommand());
            this.Register("roombadge", new RoomBadgeCommand());
            this.Register("roomalert", new RoomAlertCommand());
            this.Register("roomkick", new RoomKickCommand());
            this.Register("mute", new MuteCommand());
            this.Register("smute", new MuteCommand());
            this.Register("unmute", new UnmuteCommand());
            this.Register("massbadge", new MassBadgeCommand());
            this.Register("kick", new KickCommand());
            this.Register("skick", new KickCommand());
            this.Register("ha", new HotelAlertCommand());
            this.Register("hotelalert", new HotelAlertCommand());
            this.Register("hal", new HALCommand());
            this.Register("give", new GiveCommand());
            this.Register("givebadge", new GiveBadgeCommand());
            this.Register("dc", new DisconnectCommand());
            this.Register("kill", new DisconnectCommand());
            this.Register("disconnect", new DisconnectCommand());
            this.Register("alert", new AlertCommand());
            this.Register("tradeban", new TradeBanCommand());

            this.Register("teleport", new TeleportCommand());
            this.Register("summon", new SummonCommand());
            this.Register("override", new OverrideCommand());
            this.Register("massenable", new MassEnableCommand());
            this.Register("massdance", new MassDanceCommand());
            this.Register("freeze", new FreezeCommand());
            this.Register("unfreeze", new UnFreezeCommand());
            this.Register("fastwalk", new FastwalkCommand());
            this.Register("superfastwalk", new SuperFastwalkCommand());
            this.Register("coords", new CoordsCommand());
            this.Register("alleyesonme", new AllEyesOnMeCommand());
            this.Register("allaroundme", new AllAroundMeCommand());
            this.Register("forcesit", new ForceSitCommand());

            this.Register("ignorewhispers", new IgnoreWhispersCommand());
            this.Register("forced_effects", new DisableForcedFXCommand());

            this.Register("makesay", new MakeSayCommand());
            this.Register("flaguser", new FlagUserCommand());
        }

        /// <summary>
        /// Registers the administrator set of commands.
        /// </summary>
        private void RegisterAdministrator()
        {
            this.Register("bubble", new BubbleCommand());
            this.Register("update", new UpdateCommand());
            this.Register("deletegroup", new DeleteGroupCommand());
            this.Register("carry", new CarryCommand());
            this.Register("goto", new GOTOCommand());
        }

        /// <summary>
        /// Registers a Chat Command.
        /// </summary>
        /// <param name="CommandText">Text to type for this command.</param>
        /// <param name="Command">The command to execute.</param>
        public void Register(string CommandText, IChatCommand Command)
        {
            this._commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            var Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(" ");
                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public void LogCommand(int UserId, string Data, string MachineId)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", PlusEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }

        public bool TryGetCommand(string Command, out IChatCommand IChatCommand)
        {
            return this._commands.TryGetValue(Command, out IChatCommand);
        }
    }
}
