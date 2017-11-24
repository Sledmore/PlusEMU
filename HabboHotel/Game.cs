using log4net;

using Plus.Communication.Packets;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Televisions;
using Plus.HabboHotel.Navigator;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;

using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.LandingView;

using Plus.HabboHotel.Games;

using Plus.HabboHotel.Rooms.Chat;
using Plus.HabboHotel.Talents;
using Plus.HabboHotel.Bots;
using Plus.HabboHotel.Cache;
using Plus.HabboHotel.Rewards;
using Plus.HabboHotel.Badges;
using Plus.HabboHotel.Permissions;
using Plus.HabboHotel.Subscriptions;
using System.Threading;
using System.Threading.Tasks;
using Plus.Core;
using Plus.Core.Language;

namespace Plus.HabboHotel
{
    public class Game
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Game");
        
        private readonly PacketManager _packetManager;
        private readonly GameClientManager _clientManager;
        private readonly ModerationManager _moderationManager;
        private readonly ItemDataManager _itemDataManager;
        private readonly CatalogManager _catalogManager;
        private readonly TelevisionManager _televisionManager;//TODO: Initialize from the item manager.
        private readonly NavigatorManager _navigatorManager;
        private readonly RoomManager _roomManager;
        private readonly ChatManager _chatManager;
        private readonly GroupManager _groupManager;
        private readonly QuestManager _questManager;
        private readonly AchievementManager _achievementManager;
        private readonly TalentTrackManager _talentTrackManager;
        private readonly LandingViewManager _landingViewManager;//TODO: Rename class
        private readonly GameDataManager _gameDataManager;
        private readonly ServerStatusUpdater _globalUpdater;
        private readonly BotManager _botManager;
        private readonly CacheManager _cacheManager;
        private readonly RewardManager _rewardManager;
        private readonly BadgeManager _badgeManager;
        private readonly PermissionManager _permissionManager;
        private readonly SubscriptionManager _subscriptionManager;

        private bool _cycleEnded;
        private bool _cycleActive;
        private Task _gameCycle;
        private int _cycleSleepTime = 25;

        public Game()
        {
            this._packetManager = new PacketManager();
            this._clientManager = new GameClientManager();

            this._moderationManager = new ModerationManager();
            this._moderationManager.Init();

            this._itemDataManager = new ItemDataManager();
            this._itemDataManager.Init();

            this._catalogManager = new CatalogManager();
            this._catalogManager.Init(this._itemDataManager);

            this._televisionManager = new TelevisionManager();

            this._navigatorManager = new NavigatorManager();
            this._roomManager = new RoomManager();
            this._chatManager = new ChatManager();
            this._groupManager = new GroupManager();
            this._groupManager.Init();
            this._questManager = new QuestManager();
            this._achievementManager = new AchievementManager();
            this._talentTrackManager = new TalentTrackManager();

            this._landingViewManager = new LandingViewManager();
            this._gameDataManager = new GameDataManager();

            this._globalUpdater = new ServerStatusUpdater();
            this._globalUpdater.Init();
            
            this._botManager = new BotManager();
            this._botManager.Init();

            this._cacheManager = new CacheManager();
            this._rewardManager = new RewardManager();

            this._badgeManager = new BadgeManager();
            this._badgeManager.Init();

            this._permissionManager = new PermissionManager();
            this._permissionManager.Init();

            this._subscriptionManager = new SubscriptionManager();
            this._subscriptionManager.Init();

        }

        public void StartGameLoop()
        {
            this._gameCycle = new Task(GameCycle);
            this._gameCycle.Start();

            this._cycleActive = true;
        }

        private void GameCycle()
        {
            while (this._cycleActive)
            {
                this._cycleEnded = false;

                PlusEnvironment.GetGame().GetRoomManager().OnCycle();
                PlusEnvironment.GetGame().GetClientManager().OnCycle();

                this._cycleEnded = true;
                Thread.Sleep(this._cycleSleepTime);
            }
        }

        public void StopGameLoop()
        {
            this._cycleActive = false;

            while (!this._cycleEnded)
            {
                Thread.Sleep(this._cycleSleepTime);
            }
        }

        public PacketManager GetPacketManager()
        {
            return _packetManager;
        }

        public GameClientManager GetClientManager()
        {
            return _clientManager;
        }

        public CatalogManager GetCatalog()
        {
            return _catalogManager;
        }

        public NavigatorManager GetNavigator()
        {
            return _navigatorManager;
        }

        public ItemDataManager GetItemManager()
        {
            return _itemDataManager;
        }

        public RoomManager GetRoomManager()
        {
            return _roomManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return _achievementManager;
        }

        public TalentTrackManager GetTalentTrackManager()
        {
            return _talentTrackManager;
        }

        public ModerationManager GetModerationManager()
        {
            return this._moderationManager;
        }

        public PermissionManager GetPermissionManager()
        {
            return this._permissionManager;
        }

        public SubscriptionManager GetSubscriptionManager()
        {
            return this._subscriptionManager;
        }

        public QuestManager GetQuestManager()
        {
            return this._questManager;
        }

        public GroupManager GetGroupManager()
        {
            return _groupManager;
        }
        
        public LandingViewManager GetLandingManager()
        {
            return _landingViewManager;
        }

        public TelevisionManager GetTelevisionManager()
        {
            return _televisionManager;
        }

        public ChatManager GetChatManager()
        {
            return this._chatManager;
        }

        public GameDataManager GetGameDataManager()
        {
            return this._gameDataManager;
        }

        public BotManager GetBotManager()
        {
            return this._botManager;
        }

        public CacheManager GetCacheManager()
        {
            return this._cacheManager;
        }

        public RewardManager GetRewardManager()
        {
            return this._rewardManager;
        }

        public BadgeManager GetBadgeManager()
        {
            return this._badgeManager;
        }
    }
}