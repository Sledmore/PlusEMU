using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Plus.Core;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.Users.Badges;
using Plus.HabboHotel.Users.Inventory;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Relationships;
using Plus.HabboHotel.Users.Process;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;


using Plus.HabboHotel.Users.Navigator.SavedSearches;
using Plus.HabboHotel.Users.Effects;
using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.HabboHotel.Users.Clothing;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.Chat.Commands;
using Plus.HabboHotel.Users.Permissions;
using Plus.HabboHotel.Subscriptions;
using Plus.HabboHotel.Users.Ignores;

namespace Plus.HabboHotel.Users
{
    public class Habbo
    {
        //Generic player values.
        private int _id;
        private string _username;
        private int _rank;
        private string _motto;
        private string _look;
        private string _gender;
        private string _footballLook;
        private string _footballGender;
        private int _credits;
        private int _duckets;
        private int _diamonds;
        private int _gotwPoints;
        private int _homeRoom;
        private double _lastOnline;
        private double _accountCreated;
        private List<int> _clientVolume;
        private double _lastNameChange;
        private string _machineId;
        private bool _chatPreference;
        private bool _focusPreference;
        private bool _isExpert;
        private int _vipRank;

        //Abilitys triggered by generic events.
        private bool _appearOffline;
        private bool _allowTradingRequests;
        private bool _allowUserFollowing;
        private bool _allowFriendRequests;
        private bool _allowMessengerInvites;
        private bool _allowPetSpeech;
        private bool _allowBotSpeech;
        private bool _allowPublicRoomStatus;
        private bool _allowConsoleMessages;
        private bool _allowGifts;
        private bool _allowMimic;
        private bool _receiveWhispers;
        private bool _ignorePublicWhispers;
        private bool _playingFastFood;
        private FriendBarState _friendbarState;
        private int _christmasDay;
        private int _wantsToRideHorse;
        private int _timeAFK;
        private bool _disableForcedEffects;

        //Player saving.
        private bool _disconnected;
        private bool _habboSaved;
        private bool _changingName;

        //Counters
        private double _floodTime;
        private int _friendCount;
        private double _timeMuted;
        private double _tradingLockExpiry;
        private int _bannedPhraseCount;
        private double _sessionStart;
        private int _messengerSpamCount;
        private double _messengerSpamTime;
        private int _creditsTickUpdate;

        //Room related
        private int _tentId;
        private int _hopperId;
        private bool _isHopping;
        private int _teleportId;
        private bool _isTeleporting;
        private int _teleportingRoomId;
        private bool _roomAuthOk;
        private int _currentRoomId;

        //Advertising reporting system.
        private bool _hasSpoken;
        private bool _advertisingReported;
        private double _lastAdvertiseReport;
        private bool _advertisingReportBlocked;

        //Values generated within the game.
        private bool _wiredInteraction;
        private int _questLastCompleted;
        private bool _inventoryAlert;
        private bool _ignoreBobbaFilter;
        private bool _wiredTeleporting;
        private int _customBubbleId;
        private int _tempInt;
        private bool _onHelperDuty;

        //Fastfood
        private int _fastfoodScore;

        //Just random fun stuff.
        private int _petId;

        //Anti-script placeholders.
        private DateTime _lastGiftPurchaseTime;
        private DateTime _lastMottoUpdateTime;
        private DateTime _lastClothingUpdateTime;
        private DateTime _lastForumMessageUpdateTime;

        private int _giftPurchasingWarnings;
        private int _mottoUpdateWarnings;
        private int _clothingUpdateWarnings;

        private bool _sessionGiftBlocked;
        private bool _sessionMottoBlocked;
        private bool _sessionClothingBlocked;

        public List<int> RatedRooms;

        private GameClient _client;
        private HabboStats _habboStats;
        private HabboMessenger Messenger;
        private ProcessComponent _process;
        public ArrayList FavoriteRooms;
        public Dictionary<int, int> quests;
        private BadgeComponent BadgeComponent;
        private InventoryComponent InventoryComponent;
        public Dictionary<int, Relationship> Relationships;
        public ConcurrentDictionary<string, UserAchievement> Achievements;

        private DateTime _timeCached;

        private SearchesComponent _navigatorSearches;
        private EffectsComponent _fx;
        private ClothingComponent _clothing;
        private PermissionComponent _permissions;
        private IgnoresComponent _ignores;

        private IChatCommand _iChatCommand;

        public Habbo(int Id, string Username, int Rank, string Motto, string Look, string Gender, int Credits, int ActivityPoints, int HomeRoom,
            bool HasFriendRequestsDisabled, int LastOnline, bool AppearOffline, bool HideInRoom, double CreateDate, int Diamonds,
            string machineID, string clientVolume, bool ChatPreference, bool FocusPreference, bool PetsMuted, bool BotsMuted, bool AdvertisingReportBlocked, double LastNameChange,
            int GOTWPoints, bool IgnoreInvites, double TimeMuted, double TradingLock, bool AllowGifts, int FriendBarState, bool DisableForcedEffects, bool AllowMimic, int VIPRank)
        {
            _id = Id;
            _username = Username;
            _rank = Rank;
            _motto = Motto;
            _look = Look;
            _gender = Gender.ToLower();
            _footballLook = PlusEnvironment.FilterFigure(Look.ToLower());
            _footballGender = Gender.ToLower();
            _credits = Credits;
            _duckets = ActivityPoints;
            _diamonds = Diamonds;
            _gotwPoints = GOTWPoints;
            _homeRoom = HomeRoom;
            _lastOnline = LastOnline;
            _accountCreated = CreateDate;
            _clientVolume = new List<int>();
            foreach (string Str in clientVolume.Split(','))
            {
                int Val = 0;
                if (int.TryParse(Str, out Val))
                    _clientVolume.Add(int.Parse(Str));
                else
                    _clientVolume.Add(100);
            }

            _lastNameChange = LastNameChange;
            _machineId = machineID;
            _chatPreference = ChatPreference;
            _focusPreference = FocusPreference;
            _isExpert = IsExpert == true;

            _appearOffline = AppearOffline;
            _allowTradingRequests = true;//TODO
            _allowUserFollowing = true;//TODO
            _allowFriendRequests = HasFriendRequestsDisabled;//TODO
            _allowMessengerInvites = IgnoreInvites;
            _allowPetSpeech = PetsMuted;
            _allowBotSpeech = BotsMuted;
            _allowPublicRoomStatus = HideInRoom;
            _allowConsoleMessages = true;
            _allowGifts = AllowGifts;
            _allowMimic = AllowMimic;
            _receiveWhispers = true;
            _ignorePublicWhispers = false;
            _playingFastFood = false;
            _friendbarState = FriendBarStateUtility.GetEnum(FriendBarState);
            _christmasDay = ChristmasDay;
            _wantsToRideHorse = 0;
            _timeAFK = 0;
            _disableForcedEffects = DisableForcedEffects;
            _vipRank = VIPRank;

            _disconnected = false;
            _habboSaved = false;
            _changingName = false;

            _floodTime = 0;
            _friendCount = 0;
            _timeMuted = TimeMuted;
            _timeCached = DateTime.Now;

            _tradingLockExpiry = TradingLock;
            if (_tradingLockExpiry > 0 && PlusEnvironment.GetUnixTimestamp() > TradingLockExpiry)
            {
                _tradingLockExpiry = 0;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `user_id` = '" + Id + "' LIMIT 1");
                }
            }

            _bannedPhraseCount = 0;
            _sessionStart = PlusEnvironment.GetUnixTimestamp();
            _messengerSpamCount = 0;
            _messengerSpamTime = 0;
            _creditsTickUpdate = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.currency_scheduler.tick"));

            _tentId = 0;
            _hopperId = 0;
            _isHopping = false;
            _teleportId = 0;
            _isTeleporting = false;
            _teleportingRoomId = 0;
            _roomAuthOk = false;
            _currentRoomId = 0;

            _hasSpoken = false;
            _lastAdvertiseReport = 0;
            _advertisingReported = false;
            _advertisingReportBlocked = AdvertisingReportBlocked;

            _wiredInteraction = false;
            _questLastCompleted = 0;
            _inventoryAlert = false;
            _ignoreBobbaFilter = false;
            _wiredTeleporting = false;
            _customBubbleId = 0;
            _onHelperDuty = false;
            _fastfoodScore = 0;
            _petId = 0;
            _tempInt = 0;

            _lastGiftPurchaseTime = DateTime.Now;
            _lastMottoUpdateTime = DateTime.Now;
            _lastClothingUpdateTime = DateTime.Now;
            _lastForumMessageUpdateTime = DateTime.Now;

            _giftPurchasingWarnings = 0;
            _mottoUpdateWarnings = 0;
            _clothingUpdateWarnings = 0;

            _sessionGiftBlocked = false;
            _sessionMottoBlocked = false;
            _sessionClothingBlocked = false;

            FavoriteRooms = new ArrayList();
            Achievements = new ConcurrentDictionary<string, UserAchievement>();
            Relationships = new Dictionary<int, Relationship>();
            RatedRooms = new List<int>();

            //TODO: Nope.
            InitPermissions();

            #region Stats
            DataRow StatRow = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                dbClient.AddParameter("user_id", Id);
                StatRow = dbClient.GetRow();

                if (StatRow == null)//No row, add it yo
                {
                    dbClient.RunQuery("INSERT INTO `user_stats` (`id`) VALUES ('" + Id + "')");
                    dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                    dbClient.AddParameter("user_id", Id);
                    StatRow = dbClient.GetRow();
                }

                try
                {
                    _habboStats = new HabboStats(Convert.ToInt32(StatRow["roomvisits"]), Convert.ToDouble(StatRow["onlineTime"]), Convert.ToInt32(StatRow["respect"]), Convert.ToInt32(StatRow["respectGiven"]), Convert.ToInt32(StatRow["giftsGiven"]),
                        Convert.ToInt32(StatRow["giftsReceived"]), Convert.ToInt32(StatRow["dailyRespectPoints"]), Convert.ToInt32(StatRow["dailyPetRespectPoints"]), Convert.ToInt32(StatRow["AchievementScore"]),
                        Convert.ToInt32(StatRow["quest_id"]), Convert.ToInt32(StatRow["quest_progress"]), Convert.ToInt32(StatRow["groupid"]), Convert.ToString(StatRow["respectsTimestamp"]), Convert.ToInt32(StatRow["forum_posts"]));

                    if (Convert.ToString(StatRow["respectsTimestamp"]) != DateTime.Today.ToString("MM/dd"))
                    {
                        _habboStats.RespectsTimestamp = DateTime.Today.ToString("MM/dd");
                        SubscriptionData SubData = null;

                        int DailyRespects = 10;

                        if (_permissions.HasRight("mod_tool"))
                            DailyRespects = 20;
                        else if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(VIPRank, out SubData))
                            DailyRespects = SubData.Respects;

                        _habboStats.DailyRespectPoints = DailyRespects;
                        _habboStats.DailyPetRespectPoints = DailyRespects;

                        dbClient.RunQuery("UPDATE `user_stats` SET `dailyRespectPoints` = '" + DailyRespects + "', `dailyPetRespectPoints` = '" + DailyRespects + "', `respectsTimestamp` = '" + DateTime.Today.ToString("MM/dd") + "' WHERE `id` = '" + Id + "' LIMIT 1");
                    }
                }
                catch (Exception e)
                {
                    ExceptionLogger.LogException(e);
                }
            }

            Group G = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(_habboStats.FavouriteGroupId, out G))
                _habboStats.FavouriteGroupId = 0;
            #endregion
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public int Rank
        {
            get { return _rank; }
            set { _rank = value; }
        }

        public string Motto
        {
            get { return _motto; }
            set { _motto = value; }
        }

        public string Look
        {
            get { return _look; }
            set { _look = value; }
        }

        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        public string FootballLook
        {
            get { return _footballLook; }
            set { _footballLook = value; }
        }

        public string FootballGender
        {
            get { return _footballGender; }
            set { _footballGender = value; }
        }

        public int Credits
        {
            get { return _credits; }
            set { _credits = value; }
        }

        public int Duckets
        {
            get { return _duckets; }
            set { _duckets = value; }
        }

        public int Diamonds
        {
            get { return _diamonds; }
            set { _diamonds = value; }
        }

        public int GOTWPoints
        {
            get { return _gotwPoints; }
            set { _gotwPoints = value; }
        }

        public int HomeRoom
        {
            get { return _homeRoom; }
            set { _homeRoom = value; }
        }

        public double LastOnline
        {
            get { return _lastOnline; }
            set { _lastOnline = value; }
        }

        public double AccountCreated
        {
            get { return _accountCreated; }
            set { _accountCreated = value; }
        }

        public List<int> ClientVolume
        {
            get { return _clientVolume; }
            set { _clientVolume = value; }
        }

        public double LastNameChange
        {
            get { return _lastNameChange; }
            set { _lastNameChange = value; }
        }

        public string MachineId
        {
            get { return _machineId; }
            set { _machineId = value; }
        }

        public bool ChatPreference
        {
            get { return _chatPreference; }
            set { _chatPreference = value; }
        }
        public bool FocusPreference
        {
            get { return _focusPreference; }
            set { _focusPreference = value; }
        }

        public bool IsExpert
        {
            get { return _isExpert; }
            set { _isExpert = value; }
        }

        public bool AppearOffline
        {
            get { return _appearOffline; }
            set { _appearOffline = value; }
        }

        public int VIPRank
        {
            get { return _vipRank; }
            set { _vipRank = value; }
        }

        public int TempInt
        {
            get { return _tempInt; }
            set { _tempInt = value; }
        }

        public bool AllowTradingRequests
        {
            get { return _allowTradingRequests; }
            set { _allowTradingRequests = value; }
        }

        public bool AllowUserFollowing
        {
            get { return _allowUserFollowing; }
            set { _allowUserFollowing = value; }
        }

        public bool AllowFriendRequests
        {
            get { return _allowFriendRequests; }
            set { _allowFriendRequests = value; }
        }

        public bool AllowMessengerInvites
        {
            get { return _allowMessengerInvites; }
            set { _allowMessengerInvites = value; }
        }

        public bool AllowPetSpeech
        {
            get { return _allowPetSpeech; }
            set { _allowPetSpeech = value; }
        }

        public bool AllowBotSpeech
        {
            get { return _allowBotSpeech; }
            set { _allowBotSpeech = value; }
        }

        public bool AllowPublicRoomStatus
        {
            get { return _allowPublicRoomStatus; }
            set { _allowPublicRoomStatus = value; }
        }

        public bool AllowConsoleMessages
        {
            get { return _allowConsoleMessages; }
            set { _allowConsoleMessages = value; }
        }

        public bool AllowGifts
        {
            get { return _allowGifts; }
            set { _allowGifts = value; }
        }

        public bool AllowMimic
        {
            get { return _allowMimic; }
            set { _allowMimic = value; }
        }

        public bool ReceiveWhispers
        {
            get { return _receiveWhispers; }
            set { _receiveWhispers = value; }
        }

        public bool IgnorePublicWhispers
        {
            get { return _ignorePublicWhispers; }
            set { _ignorePublicWhispers = value; }
        }

        public bool PlayingFastFood
        {
            get { return _playingFastFood; }
            set { _playingFastFood = value; }
        }

        public FriendBarState FriendbarState
        {
            get { return _friendbarState; }
            set { _friendbarState = value; }
        }

        public int ChristmasDay
        {
            get { return _christmasDay; }
            set { _christmasDay = value; }
        }

        public int WantsToRideHorse
        {
            get { return _wantsToRideHorse; }
            set { _wantsToRideHorse = value; }
        }

        public int TimeAFK
        {
            get { return _timeAFK; }
            set { _timeAFK = value; }
        }

        public bool DisableForcedEffects
        {
            get { return _disableForcedEffects; }
            set { _disableForcedEffects = value; }
        }

        public bool ChangingName
        {
            get { return _changingName; }
            set { _changingName = value; }
        }

        public int FriendCount
        {
            get { return _friendCount; }
            set { _friendCount = value; }
        }

        public double FloodTime
        {
            get { return _floodTime; }
            set { _floodTime = value; }
        }

        public int BannedPhraseCount
        {
            get { return _bannedPhraseCount; }
            set { _bannedPhraseCount = value; }
        }

        public bool RoomAuthOk
        {
            get { return _roomAuthOk; }
            set { _roomAuthOk = value; }
        }

        public int CurrentRoomId
        {
            get { return _currentRoomId; }
            set { _currentRoomId = value; }
        }

        public int QuestLastCompleted
        {
            get { return _questLastCompleted; }
            set { _questLastCompleted = value; }
        }

        public int MessengerSpamCount
        {
            get { return _messengerSpamCount; }
            set { _messengerSpamCount = value; }
        }

        public double MessengerSpamTime
        {
            get { return _messengerSpamTime; }
            set { _messengerSpamTime = value; }
        }

        public double TimeMuted
        {
            get { return _timeMuted; }
            set { _timeMuted = value; }
        }

        public double TradingLockExpiry
        {
            get { return _tradingLockExpiry; }
            set { _tradingLockExpiry = value; }
        }

        public double SessionStart
        {
            get { return _sessionStart; }
            set { _sessionStart = value; }
        }

        public int TentId
        {
            get { return _tentId; }
            set { _tentId = value; }
        }

        public int HopperId
        {
            get { return _hopperId; }
            set { _hopperId = value; }
        }

        public bool IsHopping
        {
            get { return _isHopping; }
            set { _isHopping = value; }
        }

        public int TeleporterId
        {
            get { return _teleportId; }
            set { _teleportId = value; }
        }

        public bool IsTeleporting
        {
            get { return _isTeleporting; }
            set { _isTeleporting = value; }
        }

        public int TeleportingRoomID
        {
            get { return _teleportingRoomId; }
            set { _teleportingRoomId = value; }
        }

        public bool HasSpoken
        {
            get { return _hasSpoken; }
            set { _hasSpoken = value; }
        }

        public double LastAdvertiseReport
        {
            get { return _lastAdvertiseReport; }
            set { _lastAdvertiseReport = value; }
        }

        public bool AdvertisingReported
        {
            get { return _advertisingReported; }
            set { _advertisingReported = value; }
        }

        public bool AdvertisingReportedBlocked
        {
            get { return _advertisingReportBlocked; }
            set { _advertisingReportBlocked = value; }
        }

        public bool WiredInteraction
        {
            get { return _wiredInteraction; }
            set { _wiredInteraction = value; }
        }

        public bool InventoryAlert
        {
            get { return _inventoryAlert; }
            set { _inventoryAlert = value; }
        }

        public bool IgnoreBobbaFilter
        {
            get { return _ignoreBobbaFilter; }
            set { _ignoreBobbaFilter = value; }
        }

        public bool WiredTeleporting
        {
            get { return _wiredTeleporting; }
            set { _wiredTeleporting = value; }
        }

        public int CustomBubbleId
        {
            get { return _customBubbleId; }
            set { _customBubbleId = value; }
        }

        public bool OnHelperDuty
        {
            get { return _onHelperDuty; }
            set { _onHelperDuty = value; }
        }

        public int FastfoodScore
        {
            get { return _fastfoodScore; }
            set { _fastfoodScore = value; }
        }

        public int PetId
        {
            get { return _petId; }
            set { _petId = value; }
        }

        public int CreditsUpdateTick
        {
            get { return _creditsTickUpdate; }
            set { _creditsTickUpdate = value; }
        }

        public IChatCommand IChatCommand
        {
            get { return _iChatCommand; }
            set { _iChatCommand = value; }
        }

        public DateTime LastGiftPurchaseTime
        {
            get { return _lastGiftPurchaseTime; }
            set { _lastGiftPurchaseTime = value; }
        }

        public DateTime LastMottoUpdateTime
        {
            get { return _lastMottoUpdateTime; }
            set { _lastMottoUpdateTime = value; }
        }

        public DateTime LastClothingUpdateTime
        {
            get { return _lastClothingUpdateTime; }
            set { _lastClothingUpdateTime = value; }
        }

        public DateTime LastForumMessageUpdateTime
        {
            get { return _lastForumMessageUpdateTime; }
            set { _lastForumMessageUpdateTime = value; }
        }

        public int GiftPurchasingWarnings
        {
            get { return _giftPurchasingWarnings; }
            set { _giftPurchasingWarnings = value; }
        }

        public int MottoUpdateWarnings
        {
            get { return _mottoUpdateWarnings; }
            set { _mottoUpdateWarnings = value; }
        }

        public int ClothingUpdateWarnings
        {
            get { return _clothingUpdateWarnings; }
            set { _clothingUpdateWarnings = value; }
        }

        public bool SessionGiftBlocked
        {
            get { return _sessionGiftBlocked; }
            set { _sessionGiftBlocked = value; }
        }

        public bool SessionMottoBlocked
        {
            get { return _sessionMottoBlocked; }
            set { _sessionMottoBlocked = value; }
        }

        public bool SessionClothingBlocked
        {
            get { return _sessionClothingBlocked; }
            set { _sessionClothingBlocked = value; }
        }

        public HabboStats GetStats()
        {
            return _habboStats;
        }

        public bool InRoom
        {
            get
            {
                return CurrentRoomId >= 1 && CurrentRoom != null;
            }
        }

        public Room CurrentRoom
        {
            get
            {
                if (CurrentRoomId <= 0)
                    return null;

                Room _room = null;
                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(CurrentRoomId, out _room))
                    return _room;

                return null;
            }
        }

        public bool CacheExpired()
        {
            TimeSpan Span = DateTime.Now - _timeCached;
            return (Span.TotalMinutes >= 30);
        }

        public string GetQueryString
        {
            get
            {
                _habboSaved = true;
                return "UPDATE `users` SET `online` = '0', `last_online` = '" + PlusEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + Duckets + "', `credits` = '" + Credits + "', `vip_points` = '" + Diamonds + "', `home_room` = '" + HomeRoom + "', `gotw_points` = '" + GOTWPoints + "', `time_muted` = '" + TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(_friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + _habboStats.RoomVisits + "', `onlineTime` = '" + (PlusEnvironment.GetUnixTimestamp() - SessionStart + _habboStats.OnlineTime) + "', `respect` = '" + _habboStats.Respect + "', `respectGiven` = '" + _habboStats.RespectGiven + "', `giftsGiven` = '" + _habboStats.GiftsGiven + "', `giftsReceived` = '" + _habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + _habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + _habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + _habboStats.AchievementPoints + "', `quest_id` = '" + _habboStats.QuestId + "', `quest_progress` = '" + _habboStats.QuestProgress + "', `groupid` = '" + _habboStats.FavouriteGroupId + "',`forum_posts` = '" + _habboStats.ForumPosts + "' WHERE `id` = '" + Id + "' LIMIT 1;";
            }
        }

        public bool InitProcess()
        {
            _process = new ProcessComponent();

            return _process.Init(this);
        }

        public bool InitSearches()
        {
            _navigatorSearches = new SearchesComponent();

            return _navigatorSearches.Init(this);
        }

        public bool InitFX()
        {
            _fx = new EffectsComponent();

            return _fx.Init(this);
        }

        public bool InitClothing()
        {
            _clothing = new ClothingComponent();

            return _clothing.Init(this);
        }

        public bool InitIgnores()
        {
            _ignores = new IgnoresComponent();

            return _ignores.Init(this);
        }

        private bool InitPermissions()
        {
            _permissions = new PermissionComponent();

            return _permissions.Init(this);
        }

        public void InitInformation(UserData.UserData data)
        {
            BadgeComponent = new BadgeComponent(this   , data);
            Relationships = data.Relations;
        }

        public void Init(GameClient client, UserData.UserData data)
        {
            Achievements = data.achievements;

            FavoriteRooms = new ArrayList();
            foreach (int id in data.favouritedRooms)
            {
                FavoriteRooms.Add(id);
            }

            _client = client;
            BadgeComponent = new BadgeComponent(this, data);
            InventoryComponent = new InventoryComponent(Id, client);

            quests = data.quests;

            Messenger = new HabboMessenger(Id);
            Messenger.Init(data.friends, data.requests);
            _friendCount = Convert.ToInt32(data.friends.Count);
            _disconnected = false;
            Relationships = data.Relations;

            InitSearches();
            InitFX();
            InitClothing();
            InitIgnores();
        }


        public PermissionComponent GetPermissions()
        {
            return _permissions;
        }

        public IgnoresComponent GetIgnores()
        {
            return _ignores;
        }

        public void OnDisconnect()
        {
            if (_disconnected)
                return;

            try
            {
                if (_process != null)
                    _process.Dispose();
            }
            catch { }

            _disconnected = true;

            PlusEnvironment.GetGame().GetClientManager().UnregisterClient(Id, Username);

            if (!_habboSaved)
            {
                _habboSaved = true;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `users` SET `online` = '0', `last_online` = '" + PlusEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + Duckets + "', `credits` = '" + Credits + "', `vip_points` = '" + Diamonds + "', `home_room` = '" + HomeRoom + "', `gotw_points` = '" + GOTWPoints + "', `time_muted` = '" + TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(_friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + _habboStats.RoomVisits + "', `onlineTime` = '" + (PlusEnvironment.GetUnixTimestamp() - SessionStart + _habboStats.OnlineTime) + "', `respect` = '" + _habboStats.Respect + "', `respectGiven` = '" + _habboStats.RespectGiven + "', `giftsGiven` = '" + _habboStats.GiftsGiven + "', `giftsReceived` = '" + _habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + _habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + _habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + _habboStats.AchievementPoints + "', `quest_id` = '" + _habboStats.QuestId + "', `quest_progress` = '" + _habboStats.QuestProgress + "', `groupid` = '" + _habboStats.FavouriteGroupId + "',`forum_posts` = '" + _habboStats.ForumPosts +"' WHERE `id` = '" + Id + "' LIMIT 1;");

                    if (GetPermissions().HasRight("mod_tickets"))
                        dbClient.RunQuery("UPDATE `moderation_tickets` SET `status` = 'open', `moderator_id` = '0' WHERE `status` ='picked' AND `moderator_id` = '" + Id + "'");
                }
            }

            Dispose();

            _client = null;

        }

        public void Dispose()
        {
            if (InventoryComponent != null)
                InventoryComponent.SetIdleState();

            if (InRoom && CurrentRoom != null)
                CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(_client, false, false);

            if (Messenger != null)
            {
                Messenger.AppearOffline = true;
                Messenger.Destroy();
            }

            if (_fx != null)
                _fx.Dispose();

            if (_clothing != null)
                _clothing.Dispose();

            if (_permissions != null)
                _permissions.Dispose();

            if (_ignores != null)
                _permissions.Dispose();
        }

        public void CheckCreditsTimer()
        {
            try
            {
                _creditsTickUpdate--;

                if (_creditsTickUpdate <= 0)
                {
                    int CreditUpdate = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.currency_scheduler.credit_reward"));
                    int DucketUpdate = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.currency_scheduler.ducket_reward"));

                    SubscriptionData SubData = null;
                    if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(_vipRank, out SubData))
                    {
                        CreditUpdate += SubData.Credits;
                        DucketUpdate += SubData.Duckets;
                    }

                    _credits += CreditUpdate;
                    _duckets += DucketUpdate;

                    _client.SendPacket(new CreditBalanceComposer(_credits));
                    _client.SendPacket(new HabboActivityPointNotificationComposer(_duckets, DucketUpdate));

                    CreditsUpdateTick = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.currency_scheduler.tick"));
                }
            }
            catch { }
        }

        public GameClient GetClient()
        {
            if (_client != null)
                return _client;

            return PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Id);
        }

        public HabboMessenger GetMessenger()
        {
            return Messenger;
        }

        public BadgeComponent GetBadgeComponent()
        {
            return BadgeComponent;
        }

        public InventoryComponent GetInventoryComponent()
        {
            return InventoryComponent;
        }

        public SearchesComponent GetNavigatorSearches()
        {
            return _navigatorSearches;
        }

        public EffectsComponent Effects()
        {
            return _fx;
        }

        public ClothingComponent GetClothing()
        {
            return _clothing;
        }

        public int GetQuestProgress(int p)
        {
            int progress = 0;
            quests.TryGetValue(p, out progress);
            return progress;
        }

        public UserAchievement GetAchievementData(string p)
        {
            UserAchievement achievement = null;
            Achievements.TryGetValue(p, out achievement);
            return achievement;
        }

        public void ChangeName(string Username)
        {
            LastNameChange = PlusEnvironment.GetUnixTimestamp();
            this.Username = Username;

            SaveKey("username", Username);
            SaveKey("last_change", LastNameChange.ToString());
        }

        public void SaveKey(string Key, string Value)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET " + Key + " = @value WHERE `id` = '" + Id + "' LIMIT 1;");
                dbClient.AddParameter("value", Value);
                dbClient.RunQuery();
            }
        }

        public void PrepareRoom(int id, string password)
        {
            if (GetClient() == null || GetClient().GetHabbo() == null)
                return;

            if (GetClient().GetHabbo().InRoom)
            {
                Room OldRoom = null;
                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(GetClient().GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(GetClient(), false, false);
            }

            if (GetClient().GetHabbo().IsTeleporting && GetClient().GetHabbo().TeleportingRoomID != id)
            {
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            Room Room = null;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryLoadRoom(id, out Room))
            {
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            if (Room.isCrashed)
            {
                GetClient().SendNotification("This room has crashed! :(");
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            GetClient().GetHabbo().CurrentRoomId = Room.RoomId;

            if (Room.GetRoomUserManager().userCount >= Room.UsersMax && !GetClient().GetHabbo().GetPermissions().HasRight("room_enter_full") && GetClient().GetHabbo().Id != Room.OwnerId)
            {
                GetClient().SendPacket(new CantConnectComposer(1));
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }


            if (!GetPermissions().HasRight("room_ban_override") && Room.GetBans().IsBanned(Id))
            {
                RoomAuthOk = false;
                GetClient().GetHabbo().RoomAuthOk = false;
                GetClient().SendPacket(new CantConnectComposer(4));
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            GetClient().SendPacket(new OpenConnectionComposer());
            if (!Room.CheckRights(GetClient(), true, true) && !GetClient().GetHabbo().IsTeleporting && !GetClient().GetHabbo().IsHopping)
            {
                if (Room.Access == RoomAccess.Doorbell && !GetClient().GetHabbo().GetPermissions().HasRight("room_enter_locked"))
                {
                    if (Room.UserCount > 0)
                    {
                        GetClient().SendPacket(new DoorbellComposer(""));
                        Room.SendPacket(new DoorbellComposer(GetClient().GetHabbo().Username), true);
                        return;
                    }
                    else
                    {
                        GetClient().SendPacket(new FlatAccessDeniedComposer(""));
                        GetClient().SendPacket(new CloseConnectionComposer());
                        return;
                    }
                }
                else if (Room.Access == RoomAccess.Password && !GetClient().GetHabbo().GetPermissions().HasRight("room_enter_locked"))
                {
                    if (password.ToLower() != Room.Password.ToLower() || String.IsNullOrWhiteSpace(password))
                    {
                        GetClient().SendPacket(new GenericErrorComposer(-100002));
                        GetClient().SendPacket(new CloseConnectionComposer());
                        return;
                    }
                }
            }

            if (!EnterRoom(Room))
                GetClient().SendPacket(new CloseConnectionComposer());

        }

        public bool EnterRoom(Room Room)
        {
            if (Room == null)
                GetClient().SendPacket(new CloseConnectionComposer());

            GetClient().SendPacket(new RoomReadyComposer(Room.RoomId, Room.ModelName));
            if (Room.Wallpaper != "0.0")
                GetClient().SendPacket(new RoomPropertyComposer("wallpaper", Room.Wallpaper));
            if (Room.Floor != "0.0")
                GetClient().SendPacket(new RoomPropertyComposer("floor", Room.Floor));

            GetClient().SendPacket(new RoomPropertyComposer("landscape", Room.Landscape));
            GetClient().SendPacket(new RoomRatingComposer(Room.Score, !(GetClient().GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.OwnerId == GetClient().GetHabbo().Id)));

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO user_roomvisits (user_id,room_id,entry_timestamp,exit_timestamp,hour,minute) VALUES ('" + GetClient().GetHabbo().Id + "','" + GetClient().GetHabbo().CurrentRoomId + "','" + PlusEnvironment.GetUnixTimestamp() + "','0','" + DateTime.Now.Hour + "','" + DateTime.Now.Minute + "');");// +
            }


            if (Room.OwnerId != Id)
            {
                GetClient().GetHabbo().GetStats().RoomVisits += 1;
                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(GetClient(), "ACH_RoomEntry", 1);
            }
            return true;
        }
    }
}