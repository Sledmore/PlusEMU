using Plus.HabboHotel.Rooms.Chat.Logs;
using Plus.HabboHotel.Rooms.Chat.Filter;
using Plus.HabboHotel.Rooms.Chat.Emotions;
using Plus.HabboHotel.Rooms.Chat.Commands;
using Plus.HabboHotel.Rooms.Chat.Pets.Commands;
using Plus.HabboHotel.Rooms.Chat.Pets.Locale;
using Plus.HabboHotel.Rooms.Chat.Styles;
using Serilog;

namespace Plus.HabboHotel.Rooms.Chat
{
    public sealed class ChatManager
    {
        /// <summary>
        /// Chat Emoticons.
        /// </summary>
        private ChatEmotionsManager _emotions;

        /// <summary>
        /// Chatlog Manager
        /// </summary>
        private ChatlogManager _logs;

        /// <summary>
        /// Filter Manager.
        /// </summary>
        private WordFilterManager _filter;

        /// <summary>
        /// Commands.
        /// </summary>
        private CommandManager _commands;

        /// <summary>
        /// Pet Commands.
        /// </summary>
        private PetCommandManager _petCommands;

        /// <summary>
        /// Pet Locale.
        /// </summary>
        private PetLocale _petLocale;

        /// <summary>
        /// Chat styles.
        /// </summary>
        private ChatStyleManager _chatStyles;

        /// <summary>
        /// Initializes a new instance of the ChatManager class.
        /// </summary>
        public ChatManager()
        {
            _emotions = new ChatEmotionsManager();
            _logs = new ChatlogManager();
         
            _filter = new WordFilterManager();
            _filter.Init();

            _commands = new CommandManager(":");
            _petCommands = new PetCommandManager();
            _petLocale = new PetLocale();
      
            _chatStyles = new ChatStyleManager();
            _chatStyles.Init();

            Log.Information("Chat Manager -> LOADED");
        }

        public ChatEmotionsManager GetEmotions()
        {
            return _emotions;
        }

        public ChatlogManager GetLogs()
        {
            return _logs;
        }

        public WordFilterManager GetFilter()
        {
            return _filter;
        }

        public CommandManager GetCommands()
        {
            return _commands;
        }

        public PetCommandManager GetPetCommands()
        {
            return _petCommands;
        }

        public PetLocale GetPetLocale()
        {
            return _petLocale;
        }

        public ChatStyleManager GetChatStyles()
        {
            return _chatStyles;
        }
    }
}
