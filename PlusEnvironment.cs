using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;
using Plus.Core;

using Plus.HabboHotel;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;
using Plus.Utilities;
using log4net;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Encryption.Keys;
using Plus.Communication.Encryption;

using Plus.Database.Interfaces;
using Plus.Database;
using Plus.HabboHotel.Cache.Type;
using Plus.HabboHotel.Users.UserData;
using Plus.Communication.Rcon;
using Plus.Communication.ConnectionManager;
using Plus.Core.FigureData;
using Plus.Core.Language;
using Plus.Core.Settings;

namespace Plus
{
    public static class PlusEnvironment
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.PlusEnvironment");

        public const string PrettyVersion = "Plus Emulator";
        public const string PrettyBuild = "3.4.3.0";

        private static Encoding _defaultEncoding;
        public static CultureInfo CultureInfo;

        private static Game _game;
        private static ConfigurationData _configuration;
        private static ConnectionHandling _connectionManager;
        private static LanguageManager _languageManager;
        private static SettingsManager _settingsManager;
        private static DatabaseManager _manager;
        private static RconSocket _Rcon;
        private static FigureDataManager _figureManager;

        // TODO: Get rid?
        public static bool Event = false;
        public static DateTime lastEvent;
        public static DateTime ServerStarted;

        private static readonly List<char> Allowedchars = new List<char>(new[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
                'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '.'
            });

        private static ConcurrentDictionary<int, Habbo> _usersCached = new ConcurrentDictionary<int, Habbo>();

        public static string SWFRevision = "";

        public static void Initialize()
        {
            ServerStarted = DateTime.Now;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine("                     ____  __           ________  _____  __");
            Console.WriteLine(@"                    / __ \/ /_  _______/ ____/  |/  / / / /");
            Console.WriteLine("                   / /_/ / / / / / ___/ __/ / /|_/ / / / / ");
            Console.WriteLine("                  / ____/ / /_/ (__  ) /___/ /  / / /_/ /  ");
            Console.WriteLine(@"                 /_/   /_/\__,_/____/_____/_/  /_/\____/ ");

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("                                " + PrettyVersion + " <Build " + PrettyBuild + ">");
            Console.WriteLine("                                http://PlusIndustry.com");

            Console.WriteLine("");
            Console.Title = "Loading Plus Emulator";
            _defaultEncoding = Encoding.Default;

            Console.WriteLine("");
            Console.WriteLine("");

            CultureInfo = CultureInfo.CreateSpecificCulture("en-GB");

            try
            {
                string projectSolutionPath = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));

                _configuration = new ConfigurationData(projectSolutionPath + "//Config//config.ini");

                var connectionString = new MySqlConnectionStringBuilder
                {
                    ConnectionTimeout = 10,
                    Database = GetConfig().data["db.name"],
                    DefaultCommandTimeout = 30,
                    Logging = false,
                    MaximumPoolSize = uint.Parse(GetConfig().data["db.pool.maxsize"]),
                    MinimumPoolSize = uint.Parse(GetConfig().data["db.pool.minsize"]),
                    Password = GetConfig().data["db.password"],
                    Pooling = true,
                    Port = uint.Parse(GetConfig().data["db.port"]),
                    Server = GetConfig().data["db.hostname"],
                    UserID = GetConfig().data["db.username"],
                    AllowZeroDateTime = true,
                    ConvertZeroDateTime = true,
                    SslMode = MySqlSslMode.None
                };

                _manager = new DatabaseManager(connectionString.ToString());

                if (!_manager.IsConnected())
                {
                    log.Error("Failed to Connect to the specified MySQL server.");
                    Console.ReadKey(true);
                    Environment.Exit(1);
                    return;
                }

                log.Info("Connected to Database!");

                //Reset our statistics first.
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("TRUNCATE `catalog_marketplace_data`");
                    dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `users_now` > '0';");
                    dbClient.RunQuery("UPDATE `users` SET `online` = '0' WHERE `online` = '1'");
                    dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
                }

                //Get the configuration & Game set.
                _languageManager = new LanguageManager();
                _languageManager.Init();

                _settingsManager = new SettingsManager();
                _settingsManager.Init();

                _figureManager = new FigureDataManager();
                _figureManager.Init();

                //Have our encryption ready.
                HabboEncryptionV2.Initialize(new RSAKeys());

                //Make sure Rcon is connected before we allow clients to Connect.
                _Rcon = new RconSocket(GetConfig().data["rcon.tcp.bindip"], int.Parse(GetConfig().data["rcon.tcp.port"]), GetConfig().data["rcon.tcp.allowedaddr"].Split(Convert.ToChar(";")));

                //Accept connections.
                _connectionManager = new ConnectionHandling(int.Parse(GetConfig().data["game.tcp.port"]), int.Parse(GetConfig().data["game.tcp.conlimit"]), int.Parse(GetConfig().data["game.tcp.conperip"]), GetConfig().data["game.tcp.enablenagles"].ToLower() == "true");
                _connectionManager.Init();

                _game = new Game();
                _game.StartGameLoop();

                TimeSpan TimeUsed = DateTime.Now - ServerStarted;

                Console.WriteLine();

                log.Info("EMULATOR -> READY! (" + TimeUsed.Seconds + " s, " + TimeUsed.Milliseconds + " ms)");
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (KeyNotFoundException e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            { 
                log.Error("Please check your configuration file - some values appear to be missing.");
                log.Error("Press any key to shut down ...");
         
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (InvalidOperationException e)
            {
                log.Error("Failed to initialize PlusEmulator: " + e.Message);
                log.Error("Press any key to shut down ...");
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (Exception e)
            {
                log.Error("Fatal error during startup: " + e);
                log.Error("Press a key to exit");

                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        public static bool EnumToBool(string Enum)
        {
            return (Enum == "1");
        }

        public static string BoolToEnum(bool Bool)
        {
            return (Bool == true ? "1" : "0");
        }

        public static int GetRandomNumber(int Min, int Max)
        {
            return RandomNumber.GenerateNewRandom(Min, Max);
        }

        public static double GetUnixTimestamp()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ts.TotalSeconds;
        }

        public static long Now()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            double unixTime = ts.TotalMilliseconds;
            return (long)unixTime;
        }

        public static string FilterFigure(string figure)
        {
            foreach (char character in figure)
            {
                if (!IsValid(character))
                    return "sh-3338-93.ea-1406-62.hr-831-49.ha-3331-92.hd-180-7.ch-3334-93-1408.lg-3337-92.ca-1813-62";
            }

            return figure;
        }

        private static bool IsValid(char character)
        {
            return Allowedchars.Contains(character);
        }

        public static bool IsValidAlphaNumeric(string inputStr)
        {
            inputStr = inputStr.ToLower();
            if (string.IsNullOrEmpty(inputStr))
            {
                return false;
            }

            for (int i = 0; i < inputStr.Length; i++)
            {
                if (!IsValid(inputStr[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetUsernameById(int UserId)
        {
            string Name = "Unknown User";

            GameClient Client = GetGame().GetClientManager().GetClientByUserId(UserId);
            if (Client != null && Client.GetHabbo() != null)
                return Client.GetHabbo().Username;

            UserCache User = GetGame().GetCacheManager().GenerateUser(UserId);
            if (User != null)
                return User.Username;

            using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", UserId);
                Name = dbClient.GetString();
            }

            if (string.IsNullOrEmpty(Name))
                Name = "Unknown User";

            return Name;
        }

        public static Habbo GetHabboById(int UserId)
        {
            try
            {
                GameClient Client = GetGame().GetClientManager().GetClientByUserId(UserId);
                if (Client != null)
                {
                    Habbo User = Client.GetHabbo();
                    if (User != null && User.Id > 0)
                    {
                        if (_usersCached.ContainsKey(UserId))
                            _usersCached.TryRemove(UserId, out User);
                        return User;
                    }
                }
                else
                {
                    try
                    {
                        if (_usersCached.ContainsKey(UserId))
                            return _usersCached[UserId];
                        else
                        {
                            UserData data = UserDataFactory.GetUserData(UserId);
                            if (data != null)
                            {
                                Habbo Generated = data.user;
                                if (Generated != null)
                                {
                                    Generated.InitInformation(data);
                                    _usersCached.TryAdd(UserId, Generated);
                                    return Generated;
                                }
                            }
                        }
                    }
                    catch { return null; }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static Habbo GetHabboByUsername(String UserName)
        {
            try
            {
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @user LIMIT 1");
                    dbClient.AddParameter("user", UserName);
                    int id = dbClient.GetInteger();
                    if (id > 0)
                        return GetHabboById(Convert.ToInt32(id));
                }
                return null;
            }
            catch { return null; }
        }



        public static void PerformShutDown()
        {
            Console.Clear();
            log.Info("Server shutting down...");
            Console.Title = "PLUS EMULATOR: SHUTTING DOWN!";

            GetGame().GetClientManager().SendPacket(new BroadcastMessageAlertComposer(GetLanguageManager().TryGetValue("server.shutdown.message")));
            GetGame().StopGameLoop();
            Thread.Sleep(2500);
            GetConnectionManager().Destroy();//Stop listening.
            GetGame().GetPacketManager().UnregisterAll();//Unregister the packets.
            GetGame().GetPacketManager().WaitForAllToComplete();
            GetGame().GetClientManager().CloseAll();//Close all connections
            GetGame().GetRoomManager().Dispose();//Stop the game loop.

            using (IQueryAdapter dbClient = _manager.GetQueryReactor())
            {
                dbClient.RunQuery("TRUNCATE `catalog_marketplace_data`");
                dbClient.RunQuery("UPDATE `users` SET `online` = '0', `auth_ticket` = NULL");
                dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `users_now` > '0'");
                dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
            }

            log.Info("Plus Emulator has successfully shutdown.");

            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        public static ConfigurationData GetConfig()
        {
            return _configuration;
        }

        public static Encoding GetDefaultEncoding()
        {
            return _defaultEncoding;
        }

        public static ConnectionHandling GetConnectionManager()
        {
            return _connectionManager;
        }

        public static Game GetGame()
        {
            return _game;
        }

        public static RconSocket GetRconSocket()
        {
            return _Rcon;
        }

        public static FigureDataManager GetFigureManager()
        {
            return _figureManager;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return _manager;
        }

        public static LanguageManager GetLanguageManager()
        {
            return _languageManager;
        }

        public static SettingsManager GetSettingsManager()
        {
            return _settingsManager;
        }

        public static ICollection<Habbo> GetUsersCached()
        {
            return _usersCached.Values;
        }

        public static bool RemoveFromCache(int Id, out Habbo Data)
        {
            return _usersCached.TryRemove(Id, out Data);
        }
    }
}