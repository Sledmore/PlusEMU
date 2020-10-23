using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using log4net;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Incoming.Catalog;
using Plus.Communication.Packets.Incoming.Handshake;
using Plus.Communication.Packets.Incoming.Navigator;
using Plus.Communication.Packets.Incoming.Quests;
using Plus.Communication.Packets.Incoming.Rooms.Avatar;
using Plus.Communication.Packets.Incoming.Rooms.Chat;
using Plus.Communication.Packets.Incoming.Rooms.Connection;
using Plus.Communication.Packets.Incoming.Rooms.Engine;
using Plus.Communication.Packets.Incoming.Rooms.Action;
using Plus.Communication.Packets.Incoming.Users;
using Plus.Communication.Packets.Incoming.Inventory.AvatarEffects;
using Plus.Communication.Packets.Incoming.Inventory.Purse;
using Plus.Communication.Packets.Incoming.Sound;
using Plus.Communication.Packets.Incoming.Misc;
using Plus.Communication.Packets.Incoming.Inventory.Badges;
using Plus.Communication.Packets.Incoming.Avatar;
using Plus.Communication.Packets.Incoming.Inventory.Achievements;
using Plus.Communication.Packets.Incoming.Inventory.Bots;
using Plus.Communication.Packets.Incoming.Inventory.Pets;
using Plus.Communication.Packets.Incoming.LandingView;
using Plus.Communication.Packets.Incoming.Messenger;
using Plus.Communication.Packets.Incoming.Groups;

using Plus.Communication.Packets.Incoming.Rooms.Settings;
using Plus.Communication.Packets.Incoming.Rooms.AI.Pets;
using Plus.Communication.Packets.Incoming.Rooms.AI.Bots;
using Plus.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using Plus.Communication.Packets.Incoming.Rooms.Furni;
using Plus.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces;
using Plus.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions;
using Plus.Communication.Packets.Incoming.Help;
using Plus.Communication.Packets.Incoming.Rooms.FloorPlan;
using Plus.Communication.Packets.Incoming.Rooms.Furni.Wired;
using Plus.Communication.Packets.Incoming.Moderation;
using Plus.Communication.Packets.Incoming.Inventory.Furni;
using Plus.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using Plus.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using Plus.Communication.Packets.Incoming.Inventory.Trading;
using Plus.Communication.Packets.Incoming.GameCenter;
using Plus.Communication.Packets.Incoming.Marketplace;
using Plus.Communication.Packets.Incoming.Rooms.Furni.LoveLocks;
using Plus.Communication.Packets.Incoming.Talents;

namespace Plus.Communication.Packets
{
    public sealed class PacketManager
    {
        private static readonly ILog Log = LogManager.GetLogger("Plus.Communication.Packets");

        /// <summary>
        ///     Testing the Task code
        /// </summary>
        private readonly bool _ignoreTasks = true;

        /// <summary>
        ///     The maximum time a task can run for before it is considered dead
        ///     (can be used for debugging any locking issues with certain areas of code)
        /// </summary>
        private readonly int _maximumRunTimeInSec = 300; // 5 minutes

        /// <summary>
        ///     Should the handler throw errors or log and continue.
        /// </summary>
        private readonly bool _throwUserErrors = false;

        /// <summary>
        ///     The task factory which is used for running Asynchronous tasks, in this case we use it to execute packets.
        /// </summary>
        private readonly TaskFactory _eventDispatcher;

        private readonly Dictionary<int, IPacketEvent> _incomingPackets;
        private readonly Dictionary<int, string> _packetNames;

        /// <summary>
        ///     Currently running tasks to keep track of what the current load is
        /// </summary>
        private readonly ConcurrentDictionary<int, Task> _runningTasks;

        public PacketManager()
        {
            _incomingPackets = new Dictionary<int, IPacketEvent>();

            _eventDispatcher = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);
            _runningTasks = new ConcurrentDictionary<int, Task>();
            _packetNames = new Dictionary<int, string>();

            RegisterHandshake();
            RegisterLandingView();
            RegisterCatalog();
            RegisterMarketplace();
            RegisterNavigator();
            RegisterNewNavigator();
            RegisterRoomAction();
            RegisterQuests();
            RegisterRoomConnection();
            RegisterRoomChat();
            RegisterRoomEngine();
            RegisterFurni();
            RegisterUsers();
            RegisterSound();
            RegisterMisc();
            RegisterInventory();
            RegisterTalents();
            RegisterPurse();
            RegisterRoomAvatar();
            RegisterAvatar();
            RegisterMessenger();
            RegisterGroups();
            RegisterRoomSettings();
            RegisterPets();
            RegisterBots();
            RegisterHelp();
            FloorPlanEditor();
            RegisterModeration();
            RegisterGameCenter();
            RegisterNames();

        }

        public void TryExecutePacket(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            if (!_incomingPackets.TryGetValue(packet.Id, out IPacketEvent pak))
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    Log.Debug("Unhandled Packet: " + packet.Id);
                return;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (_packetNames.ContainsKey(packet.Id))
                    Log.Debug("Handled Packet: [" + packet.Id + "] " + _packetNames[packet.Id]);
                else
                    Log.Debug("Handled Packet: [" + packet.Id + "] UnnamedPacketEvent");
            }

            if (!_ignoreTasks)
                ExecutePacketAsync(session, packet, pak);
            else
                pak.Parse(session, packet);
        }

        private void ExecutePacketAsync(GameClient session, ClientPacket packet, IPacketEvent pak)
        {
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            CancellationToken token = cancelSource.Token;

            Task t = _eventDispatcher.StartNew(() =>
            {
                pak.Parse(session, packet);
                token.ThrowIfCancellationRequested();
            }, token);

            _runningTasks.TryAdd(t.Id, t);

            try
            {
                if (!t.Wait(_maximumRunTimeInSec * 1000, token))
                {
                    cancelSource.Cancel();
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.Flatten().InnerExceptions)
                {
                    if (_throwUserErrors)
                    {
                        throw e;
                    }
                    else
                    {
                        //log.Fatal("Unhandled Error: " + e.Message + " - " + e.StackTrace);
                        session.Disconnect();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                session.Disconnect();
            }
            finally
            {
                _runningTasks.TryRemove(t.Id, out Task _);

                cancelSource.Dispose();

                //log.Debug("Event took " + (DateTime.Now - Start).Milliseconds + "ms to complete.");
            }
        }

        public void WaitForAllToComplete()
        {
            foreach (Task t in _runningTasks.Values.ToList())
            {
                t.Wait();
            }
        }

        public void UnregisterAll()
        {
            _incomingPackets.Clear();
        }

        private void RegisterHandshake()
        {
            _incomingPackets.Add(ClientPacketHeader.GetClientVersionMessageEvent, new GetClientVersionEvent());
            _incomingPackets.Add(ClientPacketHeader.InitCryptoMessageEvent, new InitCryptoEvent());
            _incomingPackets.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, new GenerateSecretKeyEvent());
            _incomingPackets.Add(ClientPacketHeader.UniqueIDMessageEvent, new UniqueIdEvent());
            _incomingPackets.Add(ClientPacketHeader.SSOTicketMessageEvent, new SsoTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.InfoRetrieveMessageEvent, new InfoRetrieveEvent());
            _incomingPackets.Add(ClientPacketHeader.PingMessageEvent, new PingEvent());
        }

        private void RegisterLandingView()
        {
            _incomingPackets.Add(ClientPacketHeader.RefreshCampaignMessageEvent, new RefreshCampaignEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPromoArticlesMessageEvent, new GetPromoArticlesEvent());
        }

        private void RegisterCatalog()
        {
            _incomingPackets.Add(ClientPacketHeader.GetCatalogModeMessageEvent, new GetCatalogModeEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogIndexMessageEvent, new GetCatalogIndexEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogPageMessageEvent, new GetCatalogPageEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogOfferMessageEvent, new GetCatalogOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogMessageEvent, new PurchaseFromCatalogEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogAsGiftMessageEvent, new PurchaseFromCatalogAsGiftEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseRoomPromotionMessageEvent, new PurchaseRoomPromotionEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGiftWrappingConfigurationMessageEvent, new GetGiftWrappingConfigurationEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMarketplaceConfigurationMessageEvent, new GetMarketplaceConfigurationEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRecyclerRewardsMessageEvent, new GetRecyclerRewardsEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckPetNameMessageEvent, new CheckPetNameEvent());
            _incomingPackets.Add(ClientPacketHeader.RedeemVoucherMessageEvent, new RedeemVoucherEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSellablePetBreedsMessageEvent, new GetSellablePetBreedsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPromotableRoomsMessageEvent, new GetPromotableRoomsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogRoomPromotionMessageEvent, new GetCatalogRoomPromotionEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupFurniConfigMessageEvent, new GetGroupFurniConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckGnomeNameMessageEvent, new CheckGnomeNameEvent());
            _incomingPackets.Add(ClientPacketHeader.GetClubGiftsMessageEvent, new GetClubGiftsEvent());

        }

        private void RegisterMarketplace()
        {
            _incomingPackets.Add(ClientPacketHeader.GetOffersMessageEvent, new GetOffersEvent());
            _incomingPackets.Add(ClientPacketHeader.GetOwnOffersMessageEvent, new GetOwnOffersEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMarketplaceCanMakeOfferMessageEvent, new GetMarketplaceCanMakeOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMarketplaceItemStatsMessageEvent, new GetMarketplaceItemStatsEvent());
            _incomingPackets.Add(ClientPacketHeader.MakeOfferMessageEvent, new MakeOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelOfferMessageEvent, new CancelOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.BuyOfferMessageEvent, new BuyOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.RedeemOfferCreditsMessageEvent, new RedeemOfferCreditsEvent());
        }

        private void RegisterNavigator()
        {
            _incomingPackets.Add(ClientPacketHeader.AddFavouriteRoomMessageEvent, new AddFavouriteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.GetUserFlatCatsMessageEvent, new GetUserFlatCatsEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteFavouriteRoomMessageEvent, new RemoveFavouriteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.GoToHotelViewMessageEvent, new GoToHotelViewEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateNavigatorSettingsMessageEvent, new UpdateNavigatorSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.CanCreateRoomMessageEvent, new CanCreateRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.CreateFlatMessageEvent, new CreateFlatEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGuestRoomMessageEvent, new GetGuestRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.EditRoomPromotionMessageEvent, new EditRoomEventEvent());
            _incomingPackets.Add(ClientPacketHeader.GetEventCategoriesMessageEvent, new GetNavigatorFlatsEvent());
        }

        public void RegisterNewNavigator()
        {
            _incomingPackets.Add(ClientPacketHeader.InitializeNewNavigatorMessageEvent, new InitializeNewNavigatorEvent());
            _incomingPackets.Add(ClientPacketHeader.NavigatorSearchMessageEvent, new NavigatorSearchEvent());
            _incomingPackets.Add(ClientPacketHeader.FindRandomFriendingRoomMessageEvent, new FindRandomFriendingRoomEvent());
        }

        private void RegisterQuests()
        {
            _incomingPackets.Add(ClientPacketHeader.GetQuestListMessageEvent, new GetQuestListEvent());
            _incomingPackets.Add(ClientPacketHeader.StartQuestMessageEvent, new StartQuestEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelQuestMessageEvent, new CancelQuestEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCurrentQuestMessageEvent, new GetCurrentQuestEvent());
            _incomingPackets.Add(ClientPacketHeader.GetDailyQuestMessageEvent, new GetDailyQuestEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetCommunityGoalHallOfFameMessageEvent, new GetCommunityGoalHallOfFameEvent());
        }

        private void RegisterHelp()
        {
            _incomingPackets.Add(ClientPacketHeader.OnBullyClickMessageEvent, new OnBullyClickEvent());
            _incomingPackets.Add(ClientPacketHeader.SendBullyReportMessageEvent, new SendBullyReportEvent());
            _incomingPackets.Add(ClientPacketHeader.SubmitBullyReportMessageEvent, new SubmitBullyReportEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSanctionStatusMessageEvent, new GetSanctionStatusEvent());
        }

        private void RegisterRoomAction()
        {
            _incomingPackets.Add(ClientPacketHeader.LetUserInMessageEvent, new LetUserInEvent());
            _incomingPackets.Add(ClientPacketHeader.BanUserMessageEvent, new BanUserEvent());
            _incomingPackets.Add(ClientPacketHeader.KickUserMessageEvent, new KickUserEvent());
            _incomingPackets.Add(ClientPacketHeader.AssignRightsMessageEvent, new AssignRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveRightsMessageEvent, new RemoveRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveAllRightsMessageEvent, new RemoveAllRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.MuteUserMessageEvent, new MuteUserEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveHandItemMessageEvent, new GiveHandItemEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveMyRightsMessageEvent, new RemoveMyRightsEvent());
        }

        private void RegisterAvatar()
        {
            _incomingPackets.Add(ClientPacketHeader.GetWardrobeMessageEvent, new GetWardrobeEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWardrobeOutfitMessageEvent, new SaveWardrobeOutfitEvent());
        }

        private void RegisterRoomAvatar()
        {
            _incomingPackets.Add(ClientPacketHeader.ActionMessageEvent, new ActionEvent());
            _incomingPackets.Add(ClientPacketHeader.ApplySignMessageEvent, new ApplySignEvent());
            _incomingPackets.Add(ClientPacketHeader.DanceMessageEvent, new DanceEvent());
            _incomingPackets.Add(ClientPacketHeader.SitMessageEvent, new SitEvent());
            _incomingPackets.Add(ClientPacketHeader.ChangeMottoMessageEvent, new ChangeMottoEvent());
            _incomingPackets.Add(ClientPacketHeader.LookToMessageEvent, new LookToEvent());
            _incomingPackets.Add(ClientPacketHeader.DropHandItemMessageEvent, new DropHandItemEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveRoomScoreMessageEvent, new GiveRoomScoreEvent());
            _incomingPackets.Add(ClientPacketHeader.IgnoreUserMessageEvent, new IgnoreUserEvent());
            _incomingPackets.Add(ClientPacketHeader.UnIgnoreUserMessageEvent, new UnIgnoreUserEvent());
        }

        private void RegisterRoomConnection()
        {
            _incomingPackets.Add(ClientPacketHeader.OpenFlatConnectionMessageEvent, new OpenFlatConnectionEvent());
            _incomingPackets.Add(ClientPacketHeader.GoToFlatMessageEvent, new GoToFlatEvent());
        }

        private void RegisterRoomChat()
        {
            _incomingPackets.Add(ClientPacketHeader.ChatMessageEvent, new ChatEvent());
            _incomingPackets.Add(ClientPacketHeader.ShoutMessageEvent, new ShoutEvent());
            _incomingPackets.Add(ClientPacketHeader.WhisperMessageEvent, new WhisperEvent());
            _incomingPackets.Add(ClientPacketHeader.StartTypingMessageEvent, new StartTypingEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelTypingMessageEvent, new CancelTypingEvent());
        }

        private void RegisterRoomEngine()
        {
            _incomingPackets.Add(ClientPacketHeader.GetRoomEntryDataMessageEvent, new GetRoomEntryDataEvent());
            _incomingPackets.Add(ClientPacketHeader.GetFurnitureAliasesMessageEvent, new GetFurnitureAliasesEvent());
            _incomingPackets.Add(ClientPacketHeader.MoveAvatarMessageEvent, new MoveAvatarEvent());
            _incomingPackets.Add(ClientPacketHeader.MoveObjectMessageEvent, new MoveObjectEvent());
            _incomingPackets.Add(ClientPacketHeader.PickupObjectMessageEvent, new PickupObjectEvent());
            _incomingPackets.Add(ClientPacketHeader.MoveWallItemMessageEvent, new MoveWallItemEvent());
            _incomingPackets.Add(ClientPacketHeader.ApplyDecorationMessageEvent, new ApplyDecorationEvent());
            _incomingPackets.Add(ClientPacketHeader.PlaceObjectMessageEvent, new PlaceObjectEvent());
            _incomingPackets.Add(ClientPacketHeader.UseFurnitureMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.UseWallItemMessageEvent, new UseWallItemEvent());
        }

        private void RegisterInventory()
        {
            _incomingPackets.Add(ClientPacketHeader.InitTradeMessageEvent, new InitTradeEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingOfferItemMessageEvent, new TradingOfferItemEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingOfferItemsMessageEvent, new TradingOfferItemsEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingRemoveItemMessageEvent, new TradingRemoveItemEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingAcceptMessageEvent, new TradingAcceptEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingCancelMessageEvent, new TradingCancelEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingConfirmMessageEvent, new TradingConfirmEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingModifyMessageEvent, new TradingModifyEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingCancelConfirmMessageEvent, new TradingCancelConfirmEvent());
            _incomingPackets.Add(ClientPacketHeader.RequestFurniInventoryMessageEvent, new RequestFurniInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBadgesMessageEvent, new GetBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetAchievementsMessageEvent, new GetAchievementsEvent());
            _incomingPackets.Add(ClientPacketHeader.SetActivatedBadgesMessageEvent, new SetActivatedBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBotInventoryMessageEvent, new GetBotInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetInventoryMessageEvent, new GetPetInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.AvatarEffectActivatedMessageEvent, new AvatarEffectActivatedEvent());
            _incomingPackets.Add(ClientPacketHeader.AvatarEffectSelectedMessageEvent, new AvatarEffectSelectedEvent());
        }

        private void RegisterTalents()
        {
            _incomingPackets.Add(ClientPacketHeader.GetTalentTrackMessageEvent, new GetTalentTrackEvent());
        }

        private void RegisterPurse()
        {
            _incomingPackets.Add(ClientPacketHeader.GetCreditsInfoMessageEvent, new GetCreditsInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetHabboClubWindowMessageEvent, new GetHabboClubWindowEvent());
        }

        private void RegisterUsers()
        {
            _incomingPackets.Add(ClientPacketHeader.ScrGetUserInfoMessageEvent, new ScrGetUserInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.SetChatPreferenceMessageEvent, new SetChatPreferenceEvent());
            _incomingPackets.Add(ClientPacketHeader.SetUserFocusPreferenceEvent, new SetUserFocusPreferenceEvent());
            _incomingPackets.Add(ClientPacketHeader.SetMessengerInviteStatusMessageEvent, new SetMessengerInviteStatusEvent());
            _incomingPackets.Add(ClientPacketHeader.RespectUserMessageEvent, new RespectUserEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateFigureDataMessageEvent, new UpdateFigureDataEvent());
            _incomingPackets.Add(ClientPacketHeader.OpenPlayerProfileMessageEvent, new OpenPlayerProfileEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSelectedBadgesMessageEvent, new GetSelectedBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRelationshipsMessageEvent, new GetRelationshipsEvent());
            _incomingPackets.Add(ClientPacketHeader.SetRelationshipMessageEvent, new SetRelationshipEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckValidNameMessageEvent, new CheckValidNameEvent());
            _incomingPackets.Add(ClientPacketHeader.ChangeNameMessageEvent, new ChangeNameEvent());
            _incomingPackets.Add(ClientPacketHeader.GetHabboGroupBadgesMessageEvent, new GetHabboGroupBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetUserTagsMessageEvent, new GetUserTagsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetIgnoredUsersMessageEvent, new GetIgnoredUsersEvent());
        }

        private void RegisterSound()
        {
            _incomingPackets.Add(ClientPacketHeader.SetSoundSettingsMessageEvent, new SetSoundSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSongInfoMessageEvent, new GetSongInfoEvent());
        }

        private void RegisterMisc()
        {
            _incomingPackets.Add(ClientPacketHeader.EventTrackerMessageEvent, new EventTrackerEvent());
            _incomingPackets.Add(ClientPacketHeader.ClientVariablesMessageEvent, new ClientVariablesEvent());
            _incomingPackets.Add(ClientPacketHeader.DisconnectionMessageEvent, new DisconnectEvent());
            _incomingPackets.Add(ClientPacketHeader.LatencyTestMessageEvent, new LatencyTestEvent());
            _incomingPackets.Add(ClientPacketHeader.MemoryPerformanceMessageEvent, new MemoryPerformanceEvent());
            _incomingPackets.Add(ClientPacketHeader.SetFriendBarStateMessageEvent, new SetFriendBarStateEvent());
        }


        private void RegisterMessenger()
        {
            _incomingPackets.Add(ClientPacketHeader.MessengerInitMessageEvent, new MessengerInitEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBuddyRequestsMessageEvent, new GetBuddyRequestsEvent());
            _incomingPackets.Add(ClientPacketHeader.FollowFriendMessageEvent, new FollowFriendEvent());
            _incomingPackets.Add(ClientPacketHeader.FindNewFriendsMessageEvent, new FindNewFriendsEvent());
            _incomingPackets.Add(ClientPacketHeader.FriendListUpdateMessageEvent, new FriendListUpdateEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveBuddyMessageEvent, new RemoveBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.RequestBuddyMessageEvent, new RequestBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.SendMsgMessageEvent, new SendMsgEvent());
            _incomingPackets.Add(ClientPacketHeader.SendRoomInviteMessageEvent, new SendRoomInviteEvent());
            _incomingPackets.Add(ClientPacketHeader.HabboSearchMessageEvent, new HabboSearchEvent());
            _incomingPackets.Add(ClientPacketHeader.AcceptBuddyMessageEvent, new AcceptBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.DeclineBuddyMessageEvent, new DeclineBuddyEvent());
        }

        private void RegisterGroups()
        {
            _incomingPackets.Add(ClientPacketHeader.JoinGroupMessageEvent, new JoinGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveGroupFavouriteMessageEvent, new RemoveGroupFavouriteEvent());
            _incomingPackets.Add(ClientPacketHeader.SetGroupFavouriteMessageEvent, new SetGroupFavouriteEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupInfoMessageEvent, new GetGroupInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupMembersMessageEvent, new GetGroupMembersEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupCreationWindowMessageEvent, new GetGroupCreationWindowEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBadgeEditorPartsMessageEvent, new GetBadgeEditorPartsEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseGroupMessageEvent, new PurchaseGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupIdentityMessageEvent, new UpdateGroupIdentityEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupBadgeMessageEvent, new UpdateGroupBadgeEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupColoursMessageEvent, new UpdateGroupColoursEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupSettingsMessageEvent, new UpdateGroupSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.ManageGroupMessageEvent, new ManageGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveAdminRightsMessageEvent, new GiveAdminRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.TakeAdminRightsMessageEvent, new TakeAdminRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveGroupMemberMessageEvent, new RemoveGroupMemberEvent());
            _incomingPackets.Add(ClientPacketHeader.AcceptGroupMembershipMessageEvent, new AcceptGroupMembershipEvent());
            _incomingPackets.Add(ClientPacketHeader.DeclineGroupMembershipMessageEvent, new DeclineGroupMembershipEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteGroupMessageEvent, new DeleteGroupEvent());
        }

        private void RegisterRoomSettings()
        {
            _incomingPackets.Add(ClientPacketHeader.GetRoomSettingsMessageEvent, new GetRoomSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveRoomSettingsMessageEvent, new SaveRoomSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteRoomMessageEvent, new DeleteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.ToggleMuteToolMessageEvent, new ToggleMuteToolEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRoomFilterListMessageEvent, new GetRoomFilterListEvent());
            _incomingPackets.Add(ClientPacketHeader.ModifyRoomFilterListMessageEvent, new ModifyRoomFilterListEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRoomRightsMessageEvent, new GetRoomRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRoomBannedUsersMessageEvent, new GetRoomBannedUsersEvent());
            _incomingPackets.Add(ClientPacketHeader.UnbanUserFromRoomMessageEvent, new UnbanUserFromRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveEnforcedCategorySettingsMessageEvent, new SaveEnforcedCategorySettingsEvent());
        }

        private void RegisterPets()
        {
            _incomingPackets.Add(ClientPacketHeader.RespectPetMessageEvent, new RespectPetEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetInformationMessageEvent, new GetPetInformationEvent());
            _incomingPackets.Add(ClientPacketHeader.PickUpPetMessageEvent, new PickUpPetEvent());
            _incomingPackets.Add(ClientPacketHeader.PlacePetMessageEvent, new PlacePetEvent());
            _incomingPackets.Add(ClientPacketHeader.RideHorseMessageEvent, new RideHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.ApplyHorseEffectMessageEvent, new ApplyHorseEffectEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveSaddleFromHorseMessageEvent, new RemoveSaddleFromHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.ModifyWhoCanRideHorseMessageEvent, new ModifyWhoCanRideHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetTrainingPanelMessageEvent, new GetPetTrainingPanelEvent());
        }

        private void RegisterBots()
        {
            _incomingPackets.Add(ClientPacketHeader.PlaceBotMessageEvent, new PlaceBotEvent());
            _incomingPackets.Add(ClientPacketHeader.PickUpBotMessageEvent, new PickUpBotEvent());
            _incomingPackets.Add(ClientPacketHeader.OpenBotActionMessageEvent, new OpenBotActionEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveBotActionMessageEvent, new SaveBotActionEvent());
        }

        private void RegisterFurni()
        {
            _incomingPackets.Add(ClientPacketHeader.UpdateMagicTileMessageEvent, new UpdateMagicTileEvent());
            _incomingPackets.Add(ClientPacketHeader.GetYouTubeTelevisionMessageEvent, new GetYouTubeTelevisionEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRentableSpaceMessageEvent, new GetRentableSpaceEvent());
            _incomingPackets.Add(ClientPacketHeader.ToggleYouTubeVideoMessageEvent, new ToggleYouTubeVideoEvent());
            _incomingPackets.Add(ClientPacketHeader.YouTubeVideoInformationMessageEvent, new YouTubeVideoInformationEvent());
            _incomingPackets.Add(ClientPacketHeader.YouTubeGetNextVideo, new YouTubeGetNextVideo());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredTriggeRconfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredEffectConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredConditionConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveBrandingItemMessageEvent, new SaveBrandingItemEvent());
            _incomingPackets.Add(ClientPacketHeader.SetTonerMessageEvent, new SetTonerEvent());
            _incomingPackets.Add(ClientPacketHeader.DiceOffMessageEvent, new DiceOffEvent());
            _incomingPackets.Add(ClientPacketHeader.ThrowDiceMessageEvent, new ThrowDiceEvent());
            _incomingPackets.Add(ClientPacketHeader.SetMannequinNameMessageEvent, new SetMannequinNameEvent());
            _incomingPackets.Add(ClientPacketHeader.SetMannequinFigureMessageEvent, new SetMannequinFigureEvent());
            _incomingPackets.Add(ClientPacketHeader.CreditFurniRedeemMessageEvent, new CreditFurniRedeemEvent());
            _incomingPackets.Add(ClientPacketHeader.GetStickyNoteMessageEvent, new GetStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.AddStickyNoteMessageEvent, new AddStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateStickyNoteMessageEvent, new UpdateStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteStickyNoteMessageEvent, new DeleteStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMoodlightConfigMessageEvent, new GetMoodlightConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.MoodlightUpdateMessageEvent, new MoodlightUpdateEvent());
            _incomingPackets.Add(ClientPacketHeader.ToggleMoodlightMessageEvent, new ToggleMoodlightEvent());
            _incomingPackets.Add(ClientPacketHeader.UseOneWayGateMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.UseHabboWheelMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.OpenGiftMessageEvent, new OpenGiftEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupFurniSettingsMessageEvent, new GetGroupFurniSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.UseSellableClothingMessageEvent, new UseSellableClothingEvent());
            _incomingPackets.Add(ClientPacketHeader.ConfirmLoveLockMessageEvent, new ConfirmLoveLockEvent());
        }

        private void FloorPlanEditor()
        {
            _incomingPackets.Add(ClientPacketHeader.SaveFloorPlanModelMessageEvent, new SaveFloorPlanModelEvent());
            _incomingPackets.Add(ClientPacketHeader.InitializeFloorPlanSessionMessageEvent, new InitializeFloorPlanSessionEvent());
            _incomingPackets.Add(ClientPacketHeader.FloorPlanEditorRoomPropertiesMessageEvent, new FloorPlanEditorRoomPropertiesEvent());
        }

        private void RegisterModeration()
        {
            _incomingPackets.Add(ClientPacketHeader.OpenHelpToolMessageEvent, new OpenHelpToolEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorRoomInfoMessageEvent, new GetModeratorRoomInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserInfoMessageEvent, new GetModeratorUserInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserRoomVisitsMessageEvent, new GetModeratorUserRoomVisitsEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerateRoomMessageEvent, new ModerateRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.ModeratorActionMessageEvent, new ModeratorActionEvent());
            _incomingPackets.Add(ClientPacketHeader.SubmitNewTicketMessageEvent, new SubmitNewTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorRoomChatlogMessageEvent, new GetModeratorRoomChatlogEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserChatlogMessageEvent, new GetModeratorUserChatlogEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorTicketChatlogsMessageEvent, new GetModeratorTicketChatlogsEvent());
            _incomingPackets.Add(ClientPacketHeader.PickTicketMessageEvent, new PickTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.ReleaseTicketMessageEvent, new ReleaseTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.CloseTicketMesageEvent, new CloseTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationMuteMessageEvent, new ModerationMuteEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationKickMessageEvent, new ModerationKickEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationBanMessageEvent, new ModerationBanEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationMsgMessageEvent, new ModerationMsgEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationCautionMessageEvent, new ModerationCautionEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationTradeLockMessageEvent, new ModerationTradeLockEvent());
            _incomingPackets.Add(ClientPacketHeader.CallForHelpPendingCallsDeletedMessageEvent, new CallForHelpPendingCallsDeletedEvent());
            _incomingPackets.Add(ClientPacketHeader.CloseIssueDefaultActionEvent, new CloseIssueDefaultActionEvent());
        }

        public void RegisterGameCenter()
        {
            _incomingPackets.Add(ClientPacketHeader.GetGameListingMessageEvent, new GetGameListingEvent());
            _incomingPackets.Add(ClientPacketHeader.InitializeGameCenterMessageEvent, new InitializeGameCenterEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPlayableGamesMessageEvent, new GetPlayableGamesEvent());
            _incomingPackets.Add(ClientPacketHeader.JoinPlayerQueueMessageEvent, new JoinPlayerQueueEvent());
            _incomingPackets.Add(ClientPacketHeader.Game2GetWeeklyLeaderboardMessageEvent, new Game2GetWeeklyLeaderboardEvent());
        }

        public void RegisterNames()
        {
            _packetNames.Add(ClientPacketHeader.GetClientVersionMessageEvent, "GetClientVersionEvent");
            _packetNames.Add(ClientPacketHeader.InitCryptoMessageEvent, "InitCryptoEvent");
            _packetNames.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, "GenerateSecretKeyEvent");
            _packetNames.Add(ClientPacketHeader.UniqueIDMessageEvent, "UniqueIDEvent");
            _packetNames.Add(ClientPacketHeader.SSOTicketMessageEvent, "SSOTicketEvent");
            _packetNames.Add(ClientPacketHeader.InfoRetrieveMessageEvent, "InfoRetrieveEvent");
            _packetNames.Add(ClientPacketHeader.PingMessageEvent, "PingEvent");
            _packetNames.Add(ClientPacketHeader.RefreshCampaignMessageEvent, "RefreshCampaignEvent");
            _packetNames.Add(ClientPacketHeader.GetPromoArticlesMessageEvent, "RefreshPromoEvent");
            _packetNames.Add(ClientPacketHeader.GetCatalogModeMessageEvent, "GetCatalogModeEvent");
            _packetNames.Add(ClientPacketHeader.GetCatalogIndexMessageEvent, "GetCatalogIndexEvent");
            _packetNames.Add(ClientPacketHeader.GetCatalogPageMessageEvent, "GetCatalogPageEvent");
            _packetNames.Add(ClientPacketHeader.GetCatalogOfferMessageEvent, "GetCatalogOfferEvent");
            _packetNames.Add(ClientPacketHeader.PurchaseFromCatalogMessageEvent, "PurchaseFromCatalogEvent");
            _packetNames.Add(ClientPacketHeader.PurchaseFromCatalogAsGiftMessageEvent, "PurchaseFromCatalogAsGiftEvent");
            _packetNames.Add(ClientPacketHeader.PurchaseRoomPromotionMessageEvent, "PurchaseRoomPromotionEvent");
            _packetNames.Add(ClientPacketHeader.GetGiftWrappingConfigurationMessageEvent, "GetGiftWrappingConfigurationEvent");
            _packetNames.Add(ClientPacketHeader.GetMarketplaceConfigurationMessageEvent, "GetMarketplaceConfigurationEvent");
            _packetNames.Add(ClientPacketHeader.GetRecyclerRewardsMessageEvent, "GetRecyclerRewardsEvent");
            _packetNames.Add(ClientPacketHeader.CheckPetNameMessageEvent, "CheckPetNameEvent");
            _packetNames.Add(ClientPacketHeader.RedeemVoucherMessageEvent, "RedeemVoucherEvent");
            _packetNames.Add(ClientPacketHeader.GetSellablePetBreedsMessageEvent, "GetSellablePetBreedsEvent");
            _packetNames.Add(ClientPacketHeader.GetPromotableRoomsMessageEvent, "GetPromotableRoomsEvent");
            _packetNames.Add(ClientPacketHeader.GetCatalogRoomPromotionMessageEvent, "GetCatalogRoomPromotionEvent");
            _packetNames.Add(ClientPacketHeader.GetGroupFurniConfigMessageEvent, "GetGroupFurniConfigEvent");
            _packetNames.Add(ClientPacketHeader.CheckGnomeNameMessageEvent, "CheckGnomeNameEvent");
            _packetNames.Add(ClientPacketHeader.GetOffersMessageEvent, "GetOffersEvent");
            _packetNames.Add(ClientPacketHeader.GetOwnOffersMessageEvent, "GetOwnOffersEvent");
            _packetNames.Add(ClientPacketHeader.GetMarketplaceCanMakeOfferMessageEvent, "GetMarketplaceCanMakeOfferEvent");
            _packetNames.Add(ClientPacketHeader.GetMarketplaceItemStatsMessageEvent, "GetMarketplaceItemStatsEvent");
            _packetNames.Add(ClientPacketHeader.MakeOfferMessageEvent, "MakeOfferEvent");
            _packetNames.Add(ClientPacketHeader.CancelOfferMessageEvent, "CancelOfferEvent");
            _packetNames.Add(ClientPacketHeader.BuyOfferMessageEvent, "BuyOfferEvent");
            _packetNames.Add(ClientPacketHeader.RedeemOfferCreditsMessageEvent, "RedeemOfferCreditsEvent");
            _packetNames.Add(ClientPacketHeader.AddFavouriteRoomMessageEvent, "AddFavouriteRoomEvent");
            _packetNames.Add(ClientPacketHeader.GetUserFlatCatsMessageEvent, "GetUserFlatCatsEvent");
            _packetNames.Add(ClientPacketHeader.DeleteFavouriteRoomMessageEvent, "RemoveFavouriteRoomEvent");
            _packetNames.Add(ClientPacketHeader.GoToHotelViewMessageEvent, "GoToHotelViewEvent");
            _packetNames.Add(ClientPacketHeader.UpdateNavigatorSettingsMessageEvent, "UpdateNavigatorSettingsEvent");
            _packetNames.Add(ClientPacketHeader.CanCreateRoomMessageEvent, "CanCreateRoomEvent");
            _packetNames.Add(ClientPacketHeader.CreateFlatMessageEvent, "CreateFlatEvent");
            _packetNames.Add(ClientPacketHeader.GetGuestRoomMessageEvent, "GetGuestRoomEvent");
            _packetNames.Add(ClientPacketHeader.EditRoomPromotionMessageEvent, "EditRoomEventEvent");
            _packetNames.Add(ClientPacketHeader.GetEventCategoriesMessageEvent, "GetNavigatorFlatsEvent");
            _packetNames.Add(ClientPacketHeader.InitializeNewNavigatorMessageEvent, "InitializeNewNavigatorEvent");
            _packetNames.Add(ClientPacketHeader.NavigatorSearchMessageEvent, "NewNavigatorSearchEvent");
            _packetNames.Add(ClientPacketHeader.FindRandomFriendingRoomMessageEvent, "FindRandomFriendingRoomEvent");
            _packetNames.Add(ClientPacketHeader.GetQuestListMessageEvent, "GetQuestListEvent");
            _packetNames.Add(ClientPacketHeader.StartQuestMessageEvent, "StartQuestEvent");
            _packetNames.Add(ClientPacketHeader.CancelQuestMessageEvent, "CancelQuestEvent");
            _packetNames.Add(ClientPacketHeader.GetCurrentQuestMessageEvent, "GetCurrentQuestEvent");
            _packetNames.Add(ClientPacketHeader.OnBullyClickMessageEvent, "OnBullyClickEvent");
            _packetNames.Add(ClientPacketHeader.SendBullyReportMessageEvent, "SendBullyReportEvent");
            _packetNames.Add(ClientPacketHeader.SubmitBullyReportMessageEvent, "SubmitBullyReportEvent");
            _packetNames.Add(ClientPacketHeader.LetUserInMessageEvent, "LetUserInEvent");
            _packetNames.Add(ClientPacketHeader.BanUserMessageEvent, "BanUserEvent");
            _packetNames.Add(ClientPacketHeader.KickUserMessageEvent, "KickUserEvent");
            _packetNames.Add(ClientPacketHeader.AssignRightsMessageEvent, "AssignRightsEvent");
            _packetNames.Add(ClientPacketHeader.RemoveRightsMessageEvent, "RemoveRightsEvent");
            _packetNames.Add(ClientPacketHeader.RemoveAllRightsMessageEvent, "RemoveAllRightsEvent");
            _packetNames.Add(ClientPacketHeader.MuteUserMessageEvent, "MuteUserEvent");
            _packetNames.Add(ClientPacketHeader.GiveHandItemMessageEvent, "GiveHandItemEvent");
            _packetNames.Add(ClientPacketHeader.GetWardrobeMessageEvent, "GetWardrobeEvent");
            _packetNames.Add(ClientPacketHeader.SaveWardrobeOutfitMessageEvent, "SaveWardrobeOutfitEvent");
            _packetNames.Add(ClientPacketHeader.ActionMessageEvent, "ActionEvent");
            _packetNames.Add(ClientPacketHeader.ApplySignMessageEvent, "ApplySignEvent");
            _packetNames.Add(ClientPacketHeader.DanceMessageEvent, "DanceEvent");
            _packetNames.Add(ClientPacketHeader.SitMessageEvent, "SitEvent");
            _packetNames.Add(ClientPacketHeader.ChangeMottoMessageEvent, "ChangeMottoEvent");
            _packetNames.Add(ClientPacketHeader.LookToMessageEvent, "LookToEvent");
            _packetNames.Add(ClientPacketHeader.DropHandItemMessageEvent, "DropHandItemEvent");
            _packetNames.Add(ClientPacketHeader.GiveRoomScoreMessageEvent, "GiveRoomScoreEvent");
            _packetNames.Add(ClientPacketHeader.IgnoreUserMessageEvent, "IgnoreUserEvent");
            _packetNames.Add(ClientPacketHeader.UnIgnoreUserMessageEvent, "UnIgnoreUserEvent");
            _packetNames.Add(ClientPacketHeader.OpenFlatConnectionMessageEvent, "OpenFlatConnectionEvent");
            _packetNames.Add(ClientPacketHeader.GoToFlatMessageEvent, "GoToFlatEvent");
            _packetNames.Add(ClientPacketHeader.ChatMessageEvent, "ChatEvent");
            _packetNames.Add(ClientPacketHeader.ShoutMessageEvent, "ShoutEvent");
            _packetNames.Add(ClientPacketHeader.WhisperMessageEvent, "WhisperEvent");
            _packetNames.Add(ClientPacketHeader.StartTypingMessageEvent, "StartTypingEvent");
            _packetNames.Add(ClientPacketHeader.CancelTypingMessageEvent, "CancelTypingEvent");
            _packetNames.Add(ClientPacketHeader.GetRoomEntryDataMessageEvent, "GetRoomEntryDataEvent");
            _packetNames.Add(ClientPacketHeader.GetFurnitureAliasesMessageEvent, "GetFurnitureAliasesEvent");
            _packetNames.Add(ClientPacketHeader.MoveAvatarMessageEvent, "MoveAvatarEvent");
            _packetNames.Add(ClientPacketHeader.MoveObjectMessageEvent, "MoveObjectEvent");
            _packetNames.Add(ClientPacketHeader.PickupObjectMessageEvent, "PickupObjectEvent");
            _packetNames.Add(ClientPacketHeader.MoveWallItemMessageEvent, "MoveWallItemEvent");
            _packetNames.Add(ClientPacketHeader.ApplyDecorationMessageEvent, "ApplyDecorationEvent");
            _packetNames.Add(ClientPacketHeader.PlaceObjectMessageEvent, "PlaceObjectEvent");
            _packetNames.Add(ClientPacketHeader.UseFurnitureMessageEvent, "UseFurnitureEvent");
            _packetNames.Add(ClientPacketHeader.UseWallItemMessageEvent, "UseWallItemEvent");
            _packetNames.Add(ClientPacketHeader.InitTradeMessageEvent, "InitTradeEvent");
            _packetNames.Add(ClientPacketHeader.TradingOfferItemMessageEvent, "TradingOfferItemEvent");
            _packetNames.Add(ClientPacketHeader.TradingRemoveItemMessageEvent, "TradingRemoveItemEvent");
            _packetNames.Add(ClientPacketHeader.TradingAcceptMessageEvent, "TradingAcceptEvent");
            _packetNames.Add(ClientPacketHeader.TradingCancelMessageEvent, "TradingCancelEvent");
            _packetNames.Add(ClientPacketHeader.TradingConfirmMessageEvent, "TradingConfirmEvent");
            _packetNames.Add(ClientPacketHeader.TradingModifyMessageEvent, "TradingModifyEvent");
            _packetNames.Add(ClientPacketHeader.TradingCancelConfirmMessageEvent, "TradingCancelConfirmEvent");
            _packetNames.Add(ClientPacketHeader.RequestFurniInventoryMessageEvent, "RequestFurniInventoryEvent");
            _packetNames.Add(ClientPacketHeader.GetBadgesMessageEvent, "GetBadgesEvent");
            _packetNames.Add(ClientPacketHeader.GetAchievementsMessageEvent, "GetAchievementsEvent");
            _packetNames.Add(ClientPacketHeader.SetActivatedBadgesMessageEvent, "SetActivatedBadgesEvent");
            _packetNames.Add(ClientPacketHeader.GetBotInventoryMessageEvent, "GetBotInventoryEvent");
            _packetNames.Add(ClientPacketHeader.GetPetInventoryMessageEvent, "GetPetInventoryEvent");
            _packetNames.Add(ClientPacketHeader.AvatarEffectActivatedMessageEvent, "AvatarEffectActivatedEvent");
            _packetNames.Add(ClientPacketHeader.AvatarEffectSelectedMessageEvent, "AvatarEffectSelectedEvent");
            _packetNames.Add(ClientPacketHeader.GetTalentTrackMessageEvent, "GetTalentTrackEvent");
            _packetNames.Add(ClientPacketHeader.GetCreditsInfoMessageEvent, "GetCreditsInfoEvent");
            _packetNames.Add(ClientPacketHeader.GetHabboClubWindowMessageEvent, "GetHabboClubWindowEvent");
            _packetNames.Add(ClientPacketHeader.ScrGetUserInfoMessageEvent, "ScrGetUserInfoEvent");
            _packetNames.Add(ClientPacketHeader.SetChatPreferenceMessageEvent, "SetChatPreferenceEvent");
            _packetNames.Add(ClientPacketHeader.SetUserFocusPreferenceEvent, "SetUserFocusPreferenceEvent");
            _packetNames.Add(ClientPacketHeader.SetMessengerInviteStatusMessageEvent, "SetMessengerInviteStatusEvent");
            _packetNames.Add(ClientPacketHeader.RespectUserMessageEvent, "RespectUserEvent");
            _packetNames.Add(ClientPacketHeader.UpdateFigureDataMessageEvent, "UpdateFigureDataEvent");
            _packetNames.Add(ClientPacketHeader.OpenPlayerProfileMessageEvent, "OpenPlayerProfileEvent");
            _packetNames.Add(ClientPacketHeader.GetSelectedBadgesMessageEvent, "GetSelectedBadgesEvent");
            _packetNames.Add(ClientPacketHeader.GetRelationshipsMessageEvent, "GetRelationshipsEvent");
            _packetNames.Add(ClientPacketHeader.SetRelationshipMessageEvent, "SetRelationshipEvent");
            _packetNames.Add(ClientPacketHeader.CheckValidNameMessageEvent, "CheckValidNameEvent");
            _packetNames.Add(ClientPacketHeader.ChangeNameMessageEvent, "ChangeNameEvent");
            _packetNames.Add(ClientPacketHeader.GetHabboGroupBadgesMessageEvent, "GetHabboGroupBadgesEvent");
            _packetNames.Add(ClientPacketHeader.GetUserTagsMessageEvent, "GetUserTagsEvent");
            _packetNames.Add(ClientPacketHeader.SetSoundSettingsMessageEvent, "SetSoundSettingsEvent");
            _packetNames.Add(ClientPacketHeader.GetSongInfoMessageEvent, "GetSongInfoEvent");
            _packetNames.Add(ClientPacketHeader.EventTrackerMessageEvent, "EventTrackerEvent");
            _packetNames.Add(ClientPacketHeader.ClientVariablesMessageEvent, "ClientVariablesEvent");
            _packetNames.Add(ClientPacketHeader.DisconnectionMessageEvent, "DisconnectEvent");
            _packetNames.Add(ClientPacketHeader.LatencyTestMessageEvent, "LatencyTestEvent");
            _packetNames.Add(ClientPacketHeader.MemoryPerformanceMessageEvent, "MemoryPerformanceEvent");
            _packetNames.Add(ClientPacketHeader.SetFriendBarStateMessageEvent, "SetFriendBarStateEvent");
            _packetNames.Add(ClientPacketHeader.MessengerInitMessageEvent, "MessengerInitEvent");
            _packetNames.Add(ClientPacketHeader.GetBuddyRequestsMessageEvent, "GetBuddyRequestsEvent");
            _packetNames.Add(ClientPacketHeader.FollowFriendMessageEvent, "FollowFriendEvent");
            _packetNames.Add(ClientPacketHeader.FindNewFriendsMessageEvent, "FindNewFriendsEvent");
            _packetNames.Add(ClientPacketHeader.FriendListUpdateMessageEvent, "FriendListUpdateEvent");
            _packetNames.Add(ClientPacketHeader.RemoveBuddyMessageEvent, "RemoveBuddyEvent");
            _packetNames.Add(ClientPacketHeader.RequestBuddyMessageEvent, "RequestBuddyEvent");
            _packetNames.Add(ClientPacketHeader.SendMsgMessageEvent, "SendMsgEvent");
            _packetNames.Add(ClientPacketHeader.SendRoomInviteMessageEvent, "SendRoomInviteEvent");
            _packetNames.Add(ClientPacketHeader.HabboSearchMessageEvent, "HabboSearchEvent");
            _packetNames.Add(ClientPacketHeader.AcceptBuddyMessageEvent, "AcceptBuddyEvent");
            _packetNames.Add(ClientPacketHeader.DeclineBuddyMessageEvent, "DeclineBuddyEvent");
            _packetNames.Add(ClientPacketHeader.JoinGroupMessageEvent, "JoinGroupEvent");
            _packetNames.Add(ClientPacketHeader.RemoveGroupFavouriteMessageEvent, "RemoveGroupFavouriteEvent");
            _packetNames.Add(ClientPacketHeader.SetGroupFavouriteMessageEvent, "SetGroupFavouriteEvent");
            _packetNames.Add(ClientPacketHeader.GetGroupInfoMessageEvent, "GetGroupInfoEvent");
            _packetNames.Add(ClientPacketHeader.GetGroupMembersMessageEvent, "GetGroupMembersEvent");
            _packetNames.Add(ClientPacketHeader.GetGroupCreationWindowMessageEvent, "GetGroupCreationWindowEvent");
            _packetNames.Add(ClientPacketHeader.GetBadgeEditorPartsMessageEvent, "GetBadgeEditorPartsEvent");
            _packetNames.Add(ClientPacketHeader.PurchaseGroupMessageEvent, "PurchaseGroupEvent");
            _packetNames.Add(ClientPacketHeader.UpdateGroupIdentityMessageEvent, "UpdateGroupIdentityEvent");
            _packetNames.Add(ClientPacketHeader.UpdateGroupBadgeMessageEvent, "UpdateGroupBadgeEvent");
            _packetNames.Add(ClientPacketHeader.UpdateGroupColoursMessageEvent, "UpdateGroupColoursEvent");
            _packetNames.Add(ClientPacketHeader.UpdateGroupSettingsMessageEvent, "UpdateGroupSettingsEvent");
            _packetNames.Add(ClientPacketHeader.ManageGroupMessageEvent, "ManageGroupEvent");
            _packetNames.Add(ClientPacketHeader.GiveAdminRightsMessageEvent, "GiveAdminRightsEvent");
            _packetNames.Add(ClientPacketHeader.TakeAdminRightsMessageEvent, "TakeAdminRightsEvent");
            _packetNames.Add(ClientPacketHeader.RemoveGroupMemberMessageEvent, "RemoveGroupMemberEvent");
            _packetNames.Add(ClientPacketHeader.AcceptGroupMembershipMessageEvent, "AcceptGroupMembershipEvent");
            _packetNames.Add(ClientPacketHeader.DeclineGroupMembershipMessageEvent, "DeclineGroupMembershipEvent");
            _packetNames.Add(ClientPacketHeader.DeleteGroupMessageEvent, "DeleteGroupEvent");
            _packetNames.Add(ClientPacketHeader.GetForumsListDataMessageEvent, "GetForumsListDataEvent");
            _packetNames.Add(ClientPacketHeader.GetForumStatsMessageEvent, "GetForumStatsEvent");
            _packetNames.Add(ClientPacketHeader.GetThreadsListDataMessageEvent, "GetThreadsListDataEvent");
            _packetNames.Add(ClientPacketHeader.GetThreadDataMessageEvent, "GetThreadDataEvent");
            _packetNames.Add(ClientPacketHeader.GetRoomSettingsMessageEvent, "GetRoomSettingsEvent");
            _packetNames.Add(ClientPacketHeader.SaveRoomSettingsMessageEvent, "SaveRoomSettingsEvent");
            _packetNames.Add(ClientPacketHeader.DeleteRoomMessageEvent, "DeleteRoomEvent");
            _packetNames.Add(ClientPacketHeader.ToggleMuteToolMessageEvent, "ToggleMuteToolEvent");
            _packetNames.Add(ClientPacketHeader.GetRoomFilterListMessageEvent, "GetRoomFilterListEvent");
            _packetNames.Add(ClientPacketHeader.ModifyRoomFilterListMessageEvent, "ModifyRoomFilterListEvent");
            _packetNames.Add(ClientPacketHeader.GetRoomRightsMessageEvent, "GetRoomRightsEvent");
            _packetNames.Add(ClientPacketHeader.GetRoomBannedUsersMessageEvent, "GetRoomBannedUsersEvent");
            _packetNames.Add(ClientPacketHeader.UnbanUserFromRoomMessageEvent, "UnbanUserFromRoomEvent");
            _packetNames.Add(ClientPacketHeader.SaveEnforcedCategorySettingsMessageEvent, "SaveEnforcedCategorySettingsEvent");
            _packetNames.Add(ClientPacketHeader.RespectPetMessageEvent, "RespectPetEvent");
            _packetNames.Add(ClientPacketHeader.GetPetInformationMessageEvent, "GetPetInformationEvent");
            _packetNames.Add(ClientPacketHeader.PickUpPetMessageEvent, "PickUpPetEvent");
            _packetNames.Add(ClientPacketHeader.PlacePetMessageEvent, "PlacePetEvent");
            _packetNames.Add(ClientPacketHeader.RideHorseMessageEvent, "RideHorseEvent");
            _packetNames.Add(ClientPacketHeader.ApplyHorseEffectMessageEvent, "ApplyHorseEffectEvent");
            _packetNames.Add(ClientPacketHeader.RemoveSaddleFromHorseMessageEvent, "RemoveSaddleFromHorseEvent");
            _packetNames.Add(ClientPacketHeader.ModifyWhoCanRideHorseMessageEvent, "ModifyWhoCanRideHorseEvent");
            _packetNames.Add(ClientPacketHeader.GetPetTrainingPanelMessageEvent, "GetPetTrainingPanelEvent");
            _packetNames.Add(ClientPacketHeader.PlaceBotMessageEvent, "PlaceBotEvent");
            _packetNames.Add(ClientPacketHeader.PickUpBotMessageEvent, "PickUpBotEvent");
            _packetNames.Add(ClientPacketHeader.OpenBotActionMessageEvent, "OpenBotActionEvent");
            _packetNames.Add(ClientPacketHeader.SaveBotActionMessageEvent, "SaveBotActionEvent");
            _packetNames.Add(ClientPacketHeader.UpdateMagicTileMessageEvent, "UpdateMagicTileEvent");
            _packetNames.Add(ClientPacketHeader.GetYouTubeTelevisionMessageEvent, "GetYouTubeTelevisionEvent");
            _packetNames.Add(ClientPacketHeader.GetRentableSpaceMessageEvent, "GetRentableSpaceEvent");
            _packetNames.Add(ClientPacketHeader.ToggleYouTubeVideoMessageEvent, "ToggleYouTubeVideoEvent");
            _packetNames.Add(ClientPacketHeader.YouTubeVideoInformationMessageEvent, "YouTubeVideoInformationEvent");
            _packetNames.Add(ClientPacketHeader.YouTubeGetNextVideo, "YouTubeGetNextVideo");
            _packetNames.Add(ClientPacketHeader.SaveWiredTriggeRconfigMessageEvent, "SaveWiredConfigEvent");
            _packetNames.Add(ClientPacketHeader.SaveWiredEffectConfigMessageEvent, "SaveWiredConfigEvent");
            _packetNames.Add(ClientPacketHeader.SaveWiredConditionConfigMessageEvent, "SaveWiredConfigEvent");
            _packetNames.Add(ClientPacketHeader.SaveBrandingItemMessageEvent, "SaveBrandingItemEvent");
            _packetNames.Add(ClientPacketHeader.SetTonerMessageEvent, "SetTonerEvent");
            _packetNames.Add(ClientPacketHeader.DiceOffMessageEvent, "DiceOffEvent");
            _packetNames.Add(ClientPacketHeader.ThrowDiceMessageEvent, "ThrowDiceEvent");
            _packetNames.Add(ClientPacketHeader.SetMannequinNameMessageEvent, "SetMannequinNameEvent");
            _packetNames.Add(ClientPacketHeader.SetMannequinFigureMessageEvent, "SetMannequinFigureEvent");
            _packetNames.Add(ClientPacketHeader.CreditFurniRedeemMessageEvent, "CreditFurniRedeemEvent");
            _packetNames.Add(ClientPacketHeader.GetStickyNoteMessageEvent, "GetStickyNoteEvent");
            _packetNames.Add(ClientPacketHeader.AddStickyNoteMessageEvent, "AddStickyNoteEvent");
            _packetNames.Add(ClientPacketHeader.UpdateStickyNoteMessageEvent, "UpdateStickyNoteEvent");
            _packetNames.Add(ClientPacketHeader.DeleteStickyNoteMessageEvent, "DeleteStickyNoteEvent");
            _packetNames.Add(ClientPacketHeader.GetMoodlightConfigMessageEvent, "GetMoodlightConfigEvent");
            _packetNames.Add(ClientPacketHeader.MoodlightUpdateMessageEvent, "MoodlightUpdateEvent");
            _packetNames.Add(ClientPacketHeader.ToggleMoodlightMessageEvent, "ToggleMoodlightEvent");
            _packetNames.Add(ClientPacketHeader.UseOneWayGateMessageEvent, "UseFurnitureEvent");
            _packetNames.Add(ClientPacketHeader.UseHabboWheelMessageEvent, "UseFurnitureEvent");
            _packetNames.Add(ClientPacketHeader.OpenGiftMessageEvent, "OpenGiftEvent");
            _packetNames.Add(ClientPacketHeader.GetGroupFurniSettingsMessageEvent, "GetGroupFurniSettingsEvent");
            _packetNames.Add(ClientPacketHeader.UseSellableClothingMessageEvent, "UseSellableClothingEvent");
            _packetNames.Add(ClientPacketHeader.ConfirmLoveLockMessageEvent, "ConfirmLoveLockEvent");
            _packetNames.Add(ClientPacketHeader.SaveFloorPlanModelMessageEvent, "SaveFloorPlanModelEvent");
            _packetNames.Add(ClientPacketHeader.InitializeFloorPlanSessionMessageEvent, "InitializeFloorPlanSessionEvent");
            _packetNames.Add(ClientPacketHeader.FloorPlanEditorRoomPropertiesMessageEvent, "FloorPlanEditorRoomPropertiesEvent");
            _packetNames.Add(ClientPacketHeader.OpenHelpToolMessageEvent, "OpenHelpToolEvent");
            _packetNames.Add(ClientPacketHeader.GetModeratorRoomInfoMessageEvent, "GetModeratorRoomInfoEvent");
            _packetNames.Add(ClientPacketHeader.GetModeratorUserInfoMessageEvent, "GetModeratorUserInfoEvent");
            _packetNames.Add(ClientPacketHeader.GetModeratorUserRoomVisitsMessageEvent, "GetModeratorUserRoomVisitsEvent");
            _packetNames.Add(ClientPacketHeader.ModerateRoomMessageEvent, "ModerateRoomEvent");
            _packetNames.Add(ClientPacketHeader.ModeratorActionMessageEvent, "ModeratorActionEvent");
            _packetNames.Add(ClientPacketHeader.SubmitNewTicketMessageEvent, "SubmitNewTicketEvent");
            _packetNames.Add(ClientPacketHeader.GetModeratorRoomChatlogMessageEvent, "GetModeratorRoomChatlogEvent");
            _packetNames.Add(ClientPacketHeader.GetModeratorUserChatlogMessageEvent, "GetModeratorUserChatlogEvent");
            _packetNames.Add(ClientPacketHeader.GetModeratorTicketChatlogsMessageEvent, "GetModeratorTicketChatlogsEvent");
            _packetNames.Add(ClientPacketHeader.PickTicketMessageEvent, "PickTicketEvent");
            _packetNames.Add(ClientPacketHeader.ReleaseTicketMessageEvent, "ReleaseTicketEvent");
            _packetNames.Add(ClientPacketHeader.CloseTicketMesageEvent, "CloseTicketEvent");
            _packetNames.Add(ClientPacketHeader.ModerationMuteMessageEvent, "ModerationMuteEvent");
            _packetNames.Add(ClientPacketHeader.ModerationKickMessageEvent, "ModerationKickEvent");
            _packetNames.Add(ClientPacketHeader.ModerationBanMessageEvent, "ModerationBanEvent");
            _packetNames.Add(ClientPacketHeader.ModerationMsgMessageEvent, "ModerationMsgEvent");
            _packetNames.Add(ClientPacketHeader.ModerationCautionMessageEvent, "ModerationCautionEvent");
            _packetNames.Add(ClientPacketHeader.ModerationTradeLockMessageEvent, "ModerationTradeLockEvent");
            _packetNames.Add(ClientPacketHeader.GetGameListingMessageEvent, "GetGameListingEvent");
            _packetNames.Add(ClientPacketHeader.InitializeGameCenterMessageEvent, "InitializeGameCenterEvent");
            _packetNames.Add(ClientPacketHeader.GetPlayableGamesMessageEvent, "GetPlayableGamesEvent");
            _packetNames.Add(ClientPacketHeader.JoinPlayerQueueMessageEvent, "JoinPlayerQueueEvent");
            _packetNames.Add(ClientPacketHeader.Game2GetWeeklyLeaderboardMessageEvent, "Game2GetWeeklyLeaderboardEvent");
        }
    }
}