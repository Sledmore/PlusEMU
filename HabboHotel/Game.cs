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

namespace Plus.HabboHotel
{
    public class Game
    {
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
            _packetManager = new PacketManager();
            _clientManager = new GameClientManager();

            _moderationManager = new ModerationManager();
            _moderationManager.Init();

            _itemDataManager = new ItemDataManager();
            _itemDataManager.Init();

            _catalogManager = new CatalogManager();
            _catalogManager.Init(_itemDataManager);

            _televisionManager = new TelevisionManager();
            _televisionManager.Init();

            _navigatorManager = new NavigatorManager();
            _navigatorManager.Init();

            _roomManager = new RoomManager();
            _roomManager.LoadModels();

            _chatManager = new ChatManager();

            _groupManager = new GroupManager();
            _groupManager.Init();

            _questManager = new QuestManager();
            _questManager.Init();

            _achievementManager = new AchievementManager();
            _achievementManager.Init();

            _talentTrackManager = new TalentTrackManager();
            _talentTrackManager.Init();

            _landingViewManager = new LandingViewManager();
            _landingViewManager.Init();

            _gameDataManager = new GameDataManager();
            _gameDataManager.Init();

            _globalUpdater = new ServerStatusUpdater();
            _globalUpdater.Init();
            
            _botManager = new BotManager();
            _botManager.Init();

            _cacheManager = new CacheManager();

            _rewardManager = new RewardManager();
            _rewardManager.Init();

            _badgeManager = new BadgeManager();
            _badgeManager.Init();

            _permissionManager = new PermissionManager();
            _permissionManager.Init();

            _subscriptionManager = new SubscriptionManager();
            _subscriptionManager.Init();

        }

        public void StartGameLoop()
        {
            _gameCycle = new Task(GameCycle);
            _gameCycle.Start();

            _cycleActive = true;
        }

        private void GameCycle()
        {
            while (_cycleActive)
            {
                _cycleEnded = false;

                PlusEnvironment.GetGame().GetRoomManager().OnCycle();
                PlusEnvironment.GetGame().GetClientManager().OnCycle();

                _cycleEnded = true;
                Thread.Sleep(_cycleSleepTime);
            }
        }

        public void StopGameLoop()
        {
            _cycleActive = false;

            while (!_cycleEnded)
            {
                Thread.Sleep(_cycleSleepTime);
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
            return _moderationManager;
        }

        public PermissionManager GetPermissionManager()
        {
            return _permissionManager;
        }

        public SubscriptionManager GetSubscriptionManager()
        {
            return _subscriptionManager;
        }

        public QuestManager GetQuestManager()
        {
            return _questManager;
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
            return _chatManager;
        }

        public GameDataManager GetGameDataManager()
        {
            return _gameDataManager;
        }

        public BotManager GetBotManager()
        {
            return _botManager;
        }

        public CacheManager GetCacheManager()
        {
            return _cacheManager;
        }

        public RewardManager GetRewardManager()
        {
            return _rewardManager;
        }

        public BadgeManager GetBadgeManager()
        {
            return _badgeManager;
        }
    }
}