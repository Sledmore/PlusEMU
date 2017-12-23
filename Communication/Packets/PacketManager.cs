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
        private static readonly ILog log = LogManager.GetLogger("Plus.Communication.Packets");

        /// <summary>
        ///     Testing the Task code
        /// </summary>
        private readonly bool IgnoreTasks = true;

        /// <summary>
        ///     The maximum time a task can run for before it is considered dead
        ///     (can be used for debugging any locking issues with certain areas of code)
        /// </summary>
        private readonly int MaximumRunTimeInSec = 300; // 5 minutes

        /// <summary>
        ///     Should the handler throw errors or log and continue.
        /// </summary>
        private readonly bool ThrowUserErrors = false;

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
            this._incomingPackets = new Dictionary<int, IPacketEvent>();

            this._eventDispatcher = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);
            this._runningTasks = new ConcurrentDictionary<int, Task>();
            this._packetNames = new Dictionary<int, string>();

            this.RegisterHandshake();
            this.RegisterLandingView();
            this.RegisterCatalog();
            this.RegisterMarketplace();
            this.RegisterNavigator();
            this.RegisterNewNavigator();
            this.RegisterRoomAction();
            this.RegisterQuests();
            this.RegisterRoomConnection();
            this.RegisterRoomChat();
            this.RegisterRoomEngine();
            this.RegisterFurni();
            this.RegisterUsers();
            this.RegisterSound();
            this.RegisterMisc();
            this.RegisterInventory();
            this.RegisterTalents();
            this.RegisterPurse();
            this.RegisterRoomAvatar();
            this.RegisterAvatar();
            this.RegisterMessenger();
            this.RegisterGroups();
            this.RegisterRoomSettings();
            this.RegisterPets();
            this.RegisterBots();
            this.RegisterHelp();
            this.FloorPlanEditor();
            this.RegisterModeration();
            this.RegisterGameCenter();
            this.RegisterNames();

        }

        public void TryExecutePacket(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            if (!_incomingPackets.TryGetValue(packet.Id, out IPacketEvent pak))
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    log.Debug("Unhandled Packet: " + packet.ToString());
                return;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (_packetNames.ContainsKey(packet.Id))
                    log.Debug("Handled Packet: [" + packet.Id + "] " + _packetNames[packet.Id]);
                else
                    log.Debug("Handled Packet: [" + packet.Id + "] UnnamedPacketEvent");
            }

            if (!IgnoreTasks)
                ExecutePacketAsync(session, packet, pak);
            else
                pak.Parse(session, packet);
        }

        private void ExecutePacketAsync(GameClient session, ClientPacket packet, IPacketEvent pak)
        {
            DateTime start = DateTime.Now;

            CancellationTokenSource CancelSource = new CancellationTokenSource();
            CancellationToken token = CancelSource.Token;

            Task t = _eventDispatcher.StartNew(() =>
            {
                pak.Parse(session, packet);
                token.ThrowIfCancellationRequested();
            }, token);

            _runningTasks.TryAdd(t.Id, t);

            try
            {
                if (!t.Wait(MaximumRunTimeInSec * 1000, token))
                {
                    CancelSource.Cancel();
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.Flatten().InnerExceptions)
                {
                    if (ThrowUserErrors)
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
                Task RemovedTask = null;
                _runningTasks.TryRemove(t.Id, out RemovedTask);

                CancelSource.Dispose();

                //log.Debug("Event took " + (DateTime.Now - Start).Milliseconds + "ms to complete.");
            }
        }

        public void WaitForAllToComplete()
        {
            foreach (Task t in this._runningTasks.Values.ToList())
            {
                t.Wait();
            }
        }

        public void UnregisterAll()
        {
            this._incomingPackets.Clear();
        }

        private void RegisterHandshake()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetClientVersionMessageEvent, new GetClientVersionEvent());
            this._incomingPackets.Add(ClientPacketHeader.InitCryptoMessageEvent, new InitCryptoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, new GenerateSecretKeyEvent());
            this._incomingPackets.Add(ClientPacketHeader.UniqueIDMessageEvent, new UniqueIDEvent());
            this._incomingPackets.Add(ClientPacketHeader.SSOTicketMessageEvent, new SSOTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.InfoRetrieveMessageEvent, new InfoRetrieveEvent());
            this._incomingPackets.Add(ClientPacketHeader.PingMessageEvent, new PingEvent());
        }

        private void RegisterLandingView()
        {
            this._incomingPackets.Add(ClientPacketHeader.RefreshCampaignMessageEvent, new RefreshCampaignEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPromoArticlesMessageEvent, new GetPromoArticlesEvent());
        }

        private void RegisterCatalog()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogModeMessageEvent, new GetCatalogModeEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogIndexMessageEvent, new GetCatalogIndexEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogPageMessageEvent, new GetCatalogPageEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogOfferMessageEvent, new GetCatalogOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogMessageEvent, new PurchaseFromCatalogEvent());
            this._incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogAsGiftMessageEvent, new PurchaseFromCatalogAsGiftEvent());
            this._incomingPackets.Add(ClientPacketHeader.PurchaseRoomPromotionMessageEvent, new PurchaseRoomPromotionEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGiftWrappingConfigurationMessageEvent, new GetGiftWrappingConfigurationEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMarketplaceConfigurationMessageEvent, new GetMarketplaceConfigurationEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRecyclerRewardsMessageEvent, new GetRecyclerRewardsEvent());
            this._incomingPackets.Add(ClientPacketHeader.CheckPetNameMessageEvent, new CheckPetNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.RedeemVoucherMessageEvent, new RedeemVoucherEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetSellablePetBreedsMessageEvent, new GetSellablePetBreedsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPromotableRoomsMessageEvent, new GetPromotableRoomsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogRoomPromotionMessageEvent, new GetCatalogRoomPromotionEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupFurniConfigMessageEvent, new GetGroupFurniConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.CheckGnomeNameMessageEvent, new CheckGnomeNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetClubGiftsMessageEvent, new GetClubGiftsEvent());

        }

        private void RegisterMarketplace()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetOffersMessageEvent, new GetOffersEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetOwnOffersMessageEvent, new GetOwnOffersEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMarketplaceCanMakeOfferMessageEvent, new GetMarketplaceCanMakeOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMarketplaceItemStatsMessageEvent, new GetMarketplaceItemStatsEvent());
            this._incomingPackets.Add(ClientPacketHeader.MakeOfferMessageEvent, new MakeOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelOfferMessageEvent, new CancelOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.BuyOfferMessageEvent, new BuyOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.RedeemOfferCreditsMessageEvent, new RedeemOfferCreditsEvent());
        }

        private void RegisterNavigator()
        {
            this._incomingPackets.Add(ClientPacketHeader.AddFavouriteRoomMessageEvent, new AddFavouriteRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetUserFlatCatsMessageEvent, new GetUserFlatCatsEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteFavouriteRoomMessageEvent, new RemoveFavouriteRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.GoToHotelViewMessageEvent, new GoToHotelViewEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateNavigatorSettingsMessageEvent, new UpdateNavigatorSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.CanCreateRoomMessageEvent, new CanCreateRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.CreateFlatMessageEvent, new CreateFlatEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGuestRoomMessageEvent, new GetGuestRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.EditRoomPromotionMessageEvent, new EditRoomEventEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetEventCategoriesMessageEvent, new GetNavigatorFlatsEvent());
        }

        public void RegisterNewNavigator()
        {
            this._incomingPackets.Add(ClientPacketHeader.InitializeNewNavigatorMessageEvent, new InitializeNewNavigatorEvent());
            this._incomingPackets.Add(ClientPacketHeader.NavigatorSearchMessageEvent, new NavigatorSearchEvent());
            this._incomingPackets.Add(ClientPacketHeader.FindRandomFriendingRoomMessageEvent, new FindRandomFriendingRoomEvent());
        }

        private void RegisterQuests()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetQuestListMessageEvent, new GetQuestListEvent());
            this._incomingPackets.Add(ClientPacketHeader.StartQuestMessageEvent, new StartQuestEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelQuestMessageEvent, new CancelQuestEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCurrentQuestMessageEvent, new GetCurrentQuestEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetDailyQuestMessageEvent, new GetDailyQuestEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetCommunityGoalHallOfFameMessageEvent, new GetCommunityGoalHallOfFameEvent());
        }

        private void RegisterHelp()
        {
            this._incomingPackets.Add(ClientPacketHeader.OnBullyClickMessageEvent, new OnBullyClickEvent());
            this._incomingPackets.Add(ClientPacketHeader.SendBullyReportMessageEvent, new SendBullyReportEvent());
            this._incomingPackets.Add(ClientPacketHeader.SubmitBullyReportMessageEvent, new SubmitBullyReportEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetSanctionStatusMessageEvent, new GetSanctionStatusEvent());
        }

        private void RegisterRoomAction()
        {
            this._incomingPackets.Add(ClientPacketHeader.LetUserInMessageEvent, new LetUserInEvent());
            this._incomingPackets.Add(ClientPacketHeader.BanUserMessageEvent, new BanUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.KickUserMessageEvent, new KickUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.AssignRightsMessageEvent, new AssignRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveRightsMessageEvent, new RemoveRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveAllRightsMessageEvent, new RemoveAllRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.MuteUserMessageEvent, new MuteUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.GiveHandItemMessageEvent, new GiveHandItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveMyRightsMessageEvent, new RemoveMyRightsEvent());
        }

        private void RegisterAvatar()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetWardrobeMessageEvent, new GetWardrobeEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveWardrobeOutfitMessageEvent, new SaveWardrobeOutfitEvent());
        }

        private void RegisterRoomAvatar()
        {
            this._incomingPackets.Add(ClientPacketHeader.ActionMessageEvent, new ActionEvent());
            this._incomingPackets.Add(ClientPacketHeader.ApplySignMessageEvent, new ApplySignEvent());
            this._incomingPackets.Add(ClientPacketHeader.DanceMessageEvent, new DanceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SitMessageEvent, new SitEvent());
            this._incomingPackets.Add(ClientPacketHeader.ChangeMottoMessageEvent, new ChangeMottoEvent());
            this._incomingPackets.Add(ClientPacketHeader.LookToMessageEvent, new LookToEvent());
            this._incomingPackets.Add(ClientPacketHeader.DropHandItemMessageEvent, new DropHandItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.GiveRoomScoreMessageEvent, new GiveRoomScoreEvent());
            this._incomingPackets.Add(ClientPacketHeader.IgnoreUserMessageEvent, new IgnoreUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.UnIgnoreUserMessageEvent, new UnIgnoreUserEvent());
        }

        private void RegisterRoomConnection()
        {
            this._incomingPackets.Add(ClientPacketHeader.OpenFlatConnectionMessageEvent, new OpenFlatConnectionEvent());
            this._incomingPackets.Add(ClientPacketHeader.GoToFlatMessageEvent, new GoToFlatEvent());
        }

        private void RegisterRoomChat()
        {
            this._incomingPackets.Add(ClientPacketHeader.ChatMessageEvent, new ChatEvent());
            this._incomingPackets.Add(ClientPacketHeader.ShoutMessageEvent, new ShoutEvent());
            this._incomingPackets.Add(ClientPacketHeader.WhisperMessageEvent, new WhisperEvent());
            this._incomingPackets.Add(ClientPacketHeader.StartTypingMessageEvent, new StartTypingEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelTypingMessageEvent, new CancelTypingEvent());
        }

        private void RegisterRoomEngine()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetRoomEntryDataMessageEvent, new GetRoomEntryDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetFurnitureAliasesMessageEvent, new GetFurnitureAliasesEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveAvatarMessageEvent, new MoveAvatarEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveObjectMessageEvent, new MoveObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickupObjectMessageEvent, new PickupObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveWallItemMessageEvent, new MoveWallItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.ApplyDecorationMessageEvent, new ApplyDecorationEvent());
            this._incomingPackets.Add(ClientPacketHeader.PlaceObjectMessageEvent, new PlaceObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseFurnitureMessageEvent, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseWallItemMessageEvent, new UseWallItemEvent());
        }

        private void RegisterInventory()
        {
            this._incomingPackets.Add(ClientPacketHeader.InitTradeMessageEvent, new InitTradeEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingOfferItemMessageEvent, new TradingOfferItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingOfferItemsMessageEvent, new TradingOfferItemsEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingRemoveItemMessageEvent, new TradingRemoveItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingAcceptMessageEvent, new TradingAcceptEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingCancelMessageEvent, new TradingCancelEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingConfirmMessageEvent, new TradingConfirmEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingModifyMessageEvent, new TradingModifyEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingCancelConfirmMessageEvent, new TradingCancelConfirmEvent());
            this._incomingPackets.Add(ClientPacketHeader.RequestFurniInventoryMessageEvent, new RequestFurniInventoryEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetBadgesMessageEvent, new GetBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetAchievementsMessageEvent, new GetAchievementsEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetActivatedBadgesMessageEvent, new SetActivatedBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetBotInventoryMessageEvent, new GetBotInventoryEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPetInventoryMessageEvent, new GetPetInventoryEvent());
            this._incomingPackets.Add(ClientPacketHeader.AvatarEffectActivatedMessageEvent, new AvatarEffectActivatedEvent());
            this._incomingPackets.Add(ClientPacketHeader.AvatarEffectSelectedMessageEvent, new AvatarEffectSelectedEvent());
        }

        private void RegisterTalents()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetTalentTrackMessageEvent, new GetTalentTrackEvent());
        }

        private void RegisterPurse()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetCreditsInfoMessageEvent, new GetCreditsInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetHabboClubWindowMessageEvent, new GetHabboClubWindowEvent());
        }

        private void RegisterUsers()
        {
            this._incomingPackets.Add(ClientPacketHeader.ScrGetUserInfoMessageEvent, new ScrGetUserInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetChatPreferenceMessageEvent, new SetChatPreferenceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetUserFocusPreferenceEvent, new SetUserFocusPreferenceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetMessengerInviteStatusMessageEvent, new SetMessengerInviteStatusEvent());
            this._incomingPackets.Add(ClientPacketHeader.RespectUserMessageEvent, new RespectUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateFigureDataMessageEvent, new UpdateFigureDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.OpenPlayerProfileMessageEvent, new OpenPlayerProfileEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetSelectedBadgesMessageEvent, new GetSelectedBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRelationshipsMessageEvent, new GetRelationshipsEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetRelationshipMessageEvent, new SetRelationshipEvent());
            this._incomingPackets.Add(ClientPacketHeader.CheckValidNameMessageEvent, new CheckValidNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.ChangeNameMessageEvent, new ChangeNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetHabboGroupBadgesMessageEvent, new GetHabboGroupBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetUserTagsMessageEvent, new GetUserTagsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetIgnoredUsersMessageEvent, new GetIgnoredUsersEvent());
        }

        private void RegisterSound()
        {
            this._incomingPackets.Add(ClientPacketHeader.SetSoundSettingsMessageEvent, new SetSoundSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetSongInfoMessageEvent, new GetSongInfoEvent());
        }

        private void RegisterMisc()
        {
            this._incomingPackets.Add(ClientPacketHeader.EventTrackerMessageEvent, new EventTrackerEvent());
            this._incomingPackets.Add(ClientPacketHeader.ClientVariablesMessageEvent, new ClientVariablesEvent());
            this._incomingPackets.Add(ClientPacketHeader.DisconnectionMessageEvent, new DisconnectEvent());
            this._incomingPackets.Add(ClientPacketHeader.LatencyTestMessageEvent, new LatencyTestEvent());
            this._incomingPackets.Add(ClientPacketHeader.MemoryPerformanceMessageEvent, new MemoryPerformanceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetFriendBarStateMessageEvent, new SetFriendBarStateEvent());
        }


        private void RegisterMessenger()
        {
            this._incomingPackets.Add(ClientPacketHeader.MessengerInitMessageEvent, new MessengerInitEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetBuddyRequestsMessageEvent, new GetBuddyRequestsEvent());
            this._incomingPackets.Add(ClientPacketHeader.FollowFriendMessageEvent, new FollowFriendEvent());
            this._incomingPackets.Add(ClientPacketHeader.FindNewFriendsMessageEvent, new FindNewFriendsEvent());
            this._incomingPackets.Add(ClientPacketHeader.FriendListUpdateMessageEvent, new FriendListUpdateEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveBuddyMessageEvent, new RemoveBuddyEvent());
            this._incomingPackets.Add(ClientPacketHeader.RequestBuddyMessageEvent, new RequestBuddyEvent());
            this._incomingPackets.Add(ClientPacketHeader.SendMsgMessageEvent, new SendMsgEvent());
            this._incomingPackets.Add(ClientPacketHeader.SendRoomInviteMessageEvent, new SendRoomInviteEvent());
            this._incomingPackets.Add(ClientPacketHeader.HabboSearchMessageEvent, new HabboSearchEvent());
            this._incomingPackets.Add(ClientPacketHeader.AcceptBuddyMessageEvent, new AcceptBuddyEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeclineBuddyMessageEvent, new DeclineBuddyEvent());
        }

        private void RegisterGroups()
        {
            this._incomingPackets.Add(ClientPacketHeader.JoinGroupMessageEvent, new JoinGroupEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveGroupFavouriteMessageEvent, new RemoveGroupFavouriteEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetGroupFavouriteMessageEvent, new SetGroupFavouriteEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupInfoMessageEvent, new GetGroupInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupMembersMessageEvent, new GetGroupMembersEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupCreationWindowMessageEvent, new GetGroupCreationWindowEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetBadgeEditorPartsMessageEvent, new GetBadgeEditorPartsEvent());
            this._incomingPackets.Add(ClientPacketHeader.PurchaseGroupMessageEvent, new PurchaseGroupEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateGroupIdentityMessageEvent, new UpdateGroupIdentityEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateGroupBadgeMessageEvent, new UpdateGroupBadgeEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateGroupColoursMessageEvent, new UpdateGroupColoursEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateGroupSettingsMessageEvent, new UpdateGroupSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.ManageGroupMessageEvent, new ManageGroupEvent());
            this._incomingPackets.Add(ClientPacketHeader.GiveAdminRightsMessageEvent, new GiveAdminRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.TakeAdminRightsMessageEvent, new TakeAdminRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveGroupMemberMessageEvent, new RemoveGroupMemberEvent());
            this._incomingPackets.Add(ClientPacketHeader.AcceptGroupMembershipMessageEvent, new AcceptGroupMembershipEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeclineGroupMembershipMessageEvent, new DeclineGroupMembershipEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteGroupMessageEvent, new DeleteGroupEvent());
        }

        private void RegisterRoomSettings()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetRoomSettingsMessageEvent, new GetRoomSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveRoomSettingsMessageEvent, new SaveRoomSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteRoomMessageEvent, new DeleteRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.ToggleMuteToolMessageEvent, new ToggleMuteToolEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRoomFilterListMessageEvent, new GetRoomFilterListEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModifyRoomFilterListMessageEvent, new ModifyRoomFilterListEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRoomRightsMessageEvent, new GetRoomRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRoomBannedUsersMessageEvent, new GetRoomBannedUsersEvent());
            this._incomingPackets.Add(ClientPacketHeader.UnbanUserFromRoomMessageEvent, new UnbanUserFromRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveEnforcedCategorySettingsMessageEvent, new SaveEnforcedCategorySettingsEvent());
        }

        private void RegisterPets()
        {
            this._incomingPackets.Add(ClientPacketHeader.RespectPetMessageEvent, new RespectPetEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPetInformationMessageEvent, new GetPetInformationEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickUpPetMessageEvent, new PickUpPetEvent());
            this._incomingPackets.Add(ClientPacketHeader.PlacePetMessageEvent, new PlacePetEvent());
            this._incomingPackets.Add(ClientPacketHeader.RideHorseMessageEvent, new RideHorseEvent());
            this._incomingPackets.Add(ClientPacketHeader.ApplyHorseEffectMessageEvent, new ApplyHorseEffectEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveSaddleFromHorseMessageEvent, new RemoveSaddleFromHorseEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModifyWhoCanRideHorseMessageEvent, new ModifyWhoCanRideHorseEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPetTrainingPanelMessageEvent, new GetPetTrainingPanelEvent());
        }

        private void RegisterBots()
        {
            this._incomingPackets.Add(ClientPacketHeader.PlaceBotMessageEvent, new PlaceBotEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickUpBotMessageEvent, new PickUpBotEvent());
            this._incomingPackets.Add(ClientPacketHeader.OpenBotActionMessageEvent, new OpenBotActionEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveBotActionMessageEvent, new SaveBotActionEvent());
        }

        private void RegisterFurni()
        {
            this._incomingPackets.Add(ClientPacketHeader.UpdateMagicTileMessageEvent, new UpdateMagicTileEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetYouTubeTelevisionMessageEvent, new GetYouTubeTelevisionEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRentableSpaceMessageEvent, new GetRentableSpaceEvent());
            this._incomingPackets.Add(ClientPacketHeader.ToggleYouTubeVideoMessageEvent, new ToggleYouTubeVideoEvent());
            this._incomingPackets.Add(ClientPacketHeader.YouTubeVideoInformationMessageEvent, new YouTubeVideoInformationEvent());
            this._incomingPackets.Add(ClientPacketHeader.YouTubeGetNextVideo, new YouTubeGetNextVideo());
            this._incomingPackets.Add(ClientPacketHeader.SaveWiredTriggeRconfigMessageEvent, new SaveWiredConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveWiredEffectConfigMessageEvent, new SaveWiredConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveWiredConditionConfigMessageEvent, new SaveWiredConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveBrandingItemMessageEvent, new SaveBrandingItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetTonerMessageEvent, new SetTonerEvent());
            this._incomingPackets.Add(ClientPacketHeader.DiceOffMessageEvent, new DiceOffEvent());
            this._incomingPackets.Add(ClientPacketHeader.ThrowDiceMessageEvent, new ThrowDiceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetMannequinNameMessageEvent, new SetMannequinNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetMannequinFigureMessageEvent, new SetMannequinFigureEvent());
            this._incomingPackets.Add(ClientPacketHeader.CreditFurniRedeemMessageEvent, new CreditFurniRedeemEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetStickyNoteMessageEvent, new GetStickyNoteEvent());
            this._incomingPackets.Add(ClientPacketHeader.AddStickyNoteMessageEvent, new AddStickyNoteEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateStickyNoteMessageEvent, new UpdateStickyNoteEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteStickyNoteMessageEvent, new DeleteStickyNoteEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMoodlightConfigMessageEvent, new GetMoodlightConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoodlightUpdateMessageEvent, new MoodlightUpdateEvent());
            this._incomingPackets.Add(ClientPacketHeader.ToggleMoodlightMessageEvent, new ToggleMoodlightEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseOneWayGateMessageEvent, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseHabboWheelMessageEvent, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.OpenGiftMessageEvent, new OpenGiftEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupFurniSettingsMessageEvent, new GetGroupFurniSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseSellableClothingMessageEvent, new UseSellableClothingEvent());
            this._incomingPackets.Add(ClientPacketHeader.ConfirmLoveLockMessageEvent, new ConfirmLoveLockEvent());
        }

        private void FloorPlanEditor()
        {
            this._incomingPackets.Add(ClientPacketHeader.SaveFloorPlanModelMessageEvent, new SaveFloorPlanModelEvent());
            this._incomingPackets.Add(ClientPacketHeader.InitializeFloorPlanSessionMessageEvent, new InitializeFloorPlanSessionEvent());
            this._incomingPackets.Add(ClientPacketHeader.FloorPlanEditorRoomPropertiesMessageEvent, new FloorPlanEditorRoomPropertiesEvent());
        }

        private void RegisterModeration()
        {
            this._incomingPackets.Add(ClientPacketHeader.OpenHelpToolMessageEvent, new OpenHelpToolEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorRoomInfoMessageEvent, new GetModeratorRoomInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorUserInfoMessageEvent, new GetModeratorUserInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorUserRoomVisitsMessageEvent, new GetModeratorUserRoomVisitsEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerateRoomMessageEvent, new ModerateRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModeratorActionMessageEvent, new ModeratorActionEvent());
            this._incomingPackets.Add(ClientPacketHeader.SubmitNewTicketMessageEvent, new SubmitNewTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorRoomChatlogMessageEvent, new GetModeratorRoomChatlogEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorUserChatlogMessageEvent, new GetModeratorUserChatlogEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorTicketChatlogsMessageEvent, new GetModeratorTicketChatlogsEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickTicketMessageEvent, new PickTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.ReleaseTicketMessageEvent, new ReleaseTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.CloseTicketMesageEvent, new CloseTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationMuteMessageEvent, new ModerationMuteEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationKickMessageEvent, new ModerationKickEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationBanMessageEvent, new ModerationBanEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationMsgMessageEvent, new ModerationMsgEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationCautionMessageEvent, new ModerationCautionEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationTradeLockMessageEvent, new ModerationTradeLockEvent());
            this._incomingPackets.Add(ClientPacketHeader.CallForHelpPendingCallsDeletedMessageEvent, new CallForHelpPendingCallsDeletedEvent());
            this._incomingPackets.Add(ClientPacketHeader.CloseIssueDefaultActionEvent, new CloseIssueDefaultActionEvent());
        }

        public void RegisterGameCenter()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetGameListingMessageEvent, new GetGameListingEvent());
            this._incomingPackets.Add(ClientPacketHeader.InitializeGameCenterMessageEvent, new InitializeGameCenterEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPlayableGamesMessageEvent, new GetPlayableGamesEvent());
            this._incomingPackets.Add(ClientPacketHeader.JoinPlayerQueueMessageEvent, new JoinPlayerQueueEvent());
            this._incomingPackets.Add(ClientPacketHeader.Game2GetWeeklyLeaderboardMessageEvent, new Game2GetWeeklyLeaderboardEvent());
        }

        public void RegisterNames()
        {
            this._packetNames.Add(ClientPacketHeader.GetClientVersionMessageEvent, "GetClientVersionEvent");
            this._packetNames.Add(ClientPacketHeader.InitCryptoMessageEvent, "InitCryptoEvent");
            this._packetNames.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, "GenerateSecretKeyEvent");
            this._packetNames.Add(ClientPacketHeader.UniqueIDMessageEvent, "UniqueIDEvent");
            this._packetNames.Add(ClientPacketHeader.SSOTicketMessageEvent, "SSOTicketEvent");
            this._packetNames.Add(ClientPacketHeader.InfoRetrieveMessageEvent, "InfoRetrieveEvent");
            this._packetNames.Add(ClientPacketHeader.PingMessageEvent, "PingEvent");
            this._packetNames.Add(ClientPacketHeader.RefreshCampaignMessageEvent, "RefreshCampaignEvent");
            this._packetNames.Add(ClientPacketHeader.GetPromoArticlesMessageEvent, "RefreshPromoEvent");
            this._packetNames.Add(ClientPacketHeader.GetCatalogModeMessageEvent, "GetCatalogModeEvent");
            this._packetNames.Add(ClientPacketHeader.GetCatalogIndexMessageEvent, "GetCatalogIndexEvent");
            this._packetNames.Add(ClientPacketHeader.GetCatalogPageMessageEvent, "GetCatalogPageEvent");
            this._packetNames.Add(ClientPacketHeader.GetCatalogOfferMessageEvent, "GetCatalogOfferEvent");
            this._packetNames.Add(ClientPacketHeader.PurchaseFromCatalogMessageEvent, "PurchaseFromCatalogEvent");
            this._packetNames.Add(ClientPacketHeader.PurchaseFromCatalogAsGiftMessageEvent, "PurchaseFromCatalogAsGiftEvent");
            this._packetNames.Add(ClientPacketHeader.PurchaseRoomPromotionMessageEvent, "PurchaseRoomPromotionEvent");
            this._packetNames.Add(ClientPacketHeader.GetGiftWrappingConfigurationMessageEvent, "GetGiftWrappingConfigurationEvent");
            this._packetNames.Add(ClientPacketHeader.GetMarketplaceConfigurationMessageEvent, "GetMarketplaceConfigurationEvent");
            this._packetNames.Add(ClientPacketHeader.GetRecyclerRewardsMessageEvent, "GetRecyclerRewardsEvent");
            this._packetNames.Add(ClientPacketHeader.CheckPetNameMessageEvent, "CheckPetNameEvent");
            this._packetNames.Add(ClientPacketHeader.RedeemVoucherMessageEvent, "RedeemVoucherEvent");
            this._packetNames.Add(ClientPacketHeader.GetSellablePetBreedsMessageEvent, "GetSellablePetBreedsEvent");
            this._packetNames.Add(ClientPacketHeader.GetPromotableRoomsMessageEvent, "GetPromotableRoomsEvent");
            this._packetNames.Add(ClientPacketHeader.GetCatalogRoomPromotionMessageEvent, "GetCatalogRoomPromotionEvent");
            this._packetNames.Add(ClientPacketHeader.GetGroupFurniConfigMessageEvent, "GetGroupFurniConfigEvent");
            this._packetNames.Add(ClientPacketHeader.CheckGnomeNameMessageEvent, "CheckGnomeNameEvent");
            this._packetNames.Add(ClientPacketHeader.GetOffersMessageEvent, "GetOffersEvent");
            this._packetNames.Add(ClientPacketHeader.GetOwnOffersMessageEvent, "GetOwnOffersEvent");
            this._packetNames.Add(ClientPacketHeader.GetMarketplaceCanMakeOfferMessageEvent, "GetMarketplaceCanMakeOfferEvent");
            this._packetNames.Add(ClientPacketHeader.GetMarketplaceItemStatsMessageEvent, "GetMarketplaceItemStatsEvent");
            this._packetNames.Add(ClientPacketHeader.MakeOfferMessageEvent, "MakeOfferEvent");
            this._packetNames.Add(ClientPacketHeader.CancelOfferMessageEvent, "CancelOfferEvent");
            this._packetNames.Add(ClientPacketHeader.BuyOfferMessageEvent, "BuyOfferEvent");
            this._packetNames.Add(ClientPacketHeader.RedeemOfferCreditsMessageEvent, "RedeemOfferCreditsEvent");
            this._packetNames.Add(ClientPacketHeader.AddFavouriteRoomMessageEvent, "AddFavouriteRoomEvent");
            this._packetNames.Add(ClientPacketHeader.GetUserFlatCatsMessageEvent, "GetUserFlatCatsEvent");
            this._packetNames.Add(ClientPacketHeader.DeleteFavouriteRoomMessageEvent, "RemoveFavouriteRoomEvent");
            this._packetNames.Add(ClientPacketHeader.GoToHotelViewMessageEvent, "GoToHotelViewEvent");
            this._packetNames.Add(ClientPacketHeader.UpdateNavigatorSettingsMessageEvent, "UpdateNavigatorSettingsEvent");
            this._packetNames.Add(ClientPacketHeader.CanCreateRoomMessageEvent, "CanCreateRoomEvent");
            this._packetNames.Add(ClientPacketHeader.CreateFlatMessageEvent, "CreateFlatEvent");
            this._packetNames.Add(ClientPacketHeader.GetGuestRoomMessageEvent, "GetGuestRoomEvent");
            this._packetNames.Add(ClientPacketHeader.EditRoomPromotionMessageEvent, "EditRoomEventEvent");
            this._packetNames.Add(ClientPacketHeader.GetEventCategoriesMessageEvent, "GetNavigatorFlatsEvent");
            this._packetNames.Add(ClientPacketHeader.InitializeNewNavigatorMessageEvent, "InitializeNewNavigatorEvent");
            this._packetNames.Add(ClientPacketHeader.NavigatorSearchMessageEvent, "NewNavigatorSearchEvent");
            this._packetNames.Add(ClientPacketHeader.FindRandomFriendingRoomMessageEvent, "FindRandomFriendingRoomEvent");
            this._packetNames.Add(ClientPacketHeader.GetQuestListMessageEvent, "GetQuestListEvent");
            this._packetNames.Add(ClientPacketHeader.StartQuestMessageEvent, "StartQuestEvent");
            this._packetNames.Add(ClientPacketHeader.CancelQuestMessageEvent, "CancelQuestEvent");
            this._packetNames.Add(ClientPacketHeader.GetCurrentQuestMessageEvent, "GetCurrentQuestEvent");
            this._packetNames.Add(ClientPacketHeader.OnBullyClickMessageEvent, "OnBullyClickEvent");
            this._packetNames.Add(ClientPacketHeader.SendBullyReportMessageEvent, "SendBullyReportEvent");
            this._packetNames.Add(ClientPacketHeader.SubmitBullyReportMessageEvent, "SubmitBullyReportEvent");
            this._packetNames.Add(ClientPacketHeader.LetUserInMessageEvent, "LetUserInEvent");
            this._packetNames.Add(ClientPacketHeader.BanUserMessageEvent, "BanUserEvent");
            this._packetNames.Add(ClientPacketHeader.KickUserMessageEvent, "KickUserEvent");
            this._packetNames.Add(ClientPacketHeader.AssignRightsMessageEvent, "AssignRightsEvent");
            this._packetNames.Add(ClientPacketHeader.RemoveRightsMessageEvent, "RemoveRightsEvent");
            this._packetNames.Add(ClientPacketHeader.RemoveAllRightsMessageEvent, "RemoveAllRightsEvent");
            this._packetNames.Add(ClientPacketHeader.MuteUserMessageEvent, "MuteUserEvent");
            this._packetNames.Add(ClientPacketHeader.GiveHandItemMessageEvent, "GiveHandItemEvent");
            this._packetNames.Add(ClientPacketHeader.GetWardrobeMessageEvent, "GetWardrobeEvent");
            this._packetNames.Add(ClientPacketHeader.SaveWardrobeOutfitMessageEvent, "SaveWardrobeOutfitEvent");
            this._packetNames.Add(ClientPacketHeader.ActionMessageEvent, "ActionEvent");
            this._packetNames.Add(ClientPacketHeader.ApplySignMessageEvent, "ApplySignEvent");
            this._packetNames.Add(ClientPacketHeader.DanceMessageEvent, "DanceEvent");
            this._packetNames.Add(ClientPacketHeader.SitMessageEvent, "SitEvent");
            this._packetNames.Add(ClientPacketHeader.ChangeMottoMessageEvent, "ChangeMottoEvent");
            this._packetNames.Add(ClientPacketHeader.LookToMessageEvent, "LookToEvent");
            this._packetNames.Add(ClientPacketHeader.DropHandItemMessageEvent, "DropHandItemEvent");
            this._packetNames.Add(ClientPacketHeader.GiveRoomScoreMessageEvent, "GiveRoomScoreEvent");
            this._packetNames.Add(ClientPacketHeader.IgnoreUserMessageEvent, "IgnoreUserEvent");
            this._packetNames.Add(ClientPacketHeader.UnIgnoreUserMessageEvent, "UnIgnoreUserEvent");
            this._packetNames.Add(ClientPacketHeader.OpenFlatConnectionMessageEvent, "OpenFlatConnectionEvent");
            this._packetNames.Add(ClientPacketHeader.GoToFlatMessageEvent, "GoToFlatEvent");
            this._packetNames.Add(ClientPacketHeader.ChatMessageEvent, "ChatEvent");
            this._packetNames.Add(ClientPacketHeader.ShoutMessageEvent, "ShoutEvent");
            this._packetNames.Add(ClientPacketHeader.WhisperMessageEvent, "WhisperEvent");
            this._packetNames.Add(ClientPacketHeader.StartTypingMessageEvent, "StartTypingEvent");
            this._packetNames.Add(ClientPacketHeader.CancelTypingMessageEvent, "CancelTypingEvent");
            this._packetNames.Add(ClientPacketHeader.GetRoomEntryDataMessageEvent, "GetRoomEntryDataEvent");
            this._packetNames.Add(ClientPacketHeader.GetFurnitureAliasesMessageEvent, "GetFurnitureAliasesEvent");
            this._packetNames.Add(ClientPacketHeader.MoveAvatarMessageEvent, "MoveAvatarEvent");
            this._packetNames.Add(ClientPacketHeader.MoveObjectMessageEvent, "MoveObjectEvent");
            this._packetNames.Add(ClientPacketHeader.PickupObjectMessageEvent, "PickupObjectEvent");
            this._packetNames.Add(ClientPacketHeader.MoveWallItemMessageEvent, "MoveWallItemEvent");
            this._packetNames.Add(ClientPacketHeader.ApplyDecorationMessageEvent, "ApplyDecorationEvent");
            this._packetNames.Add(ClientPacketHeader.PlaceObjectMessageEvent, "PlaceObjectEvent");
            this._packetNames.Add(ClientPacketHeader.UseFurnitureMessageEvent, "UseFurnitureEvent");
            this._packetNames.Add(ClientPacketHeader.UseWallItemMessageEvent, "UseWallItemEvent");
            this._packetNames.Add(ClientPacketHeader.InitTradeMessageEvent, "InitTradeEvent");
            this._packetNames.Add(ClientPacketHeader.TradingOfferItemMessageEvent, "TradingOfferItemEvent");
            this._packetNames.Add(ClientPacketHeader.TradingRemoveItemMessageEvent, "TradingRemoveItemEvent");
            this._packetNames.Add(ClientPacketHeader.TradingAcceptMessageEvent, "TradingAcceptEvent");
            this._packetNames.Add(ClientPacketHeader.TradingCancelMessageEvent, "TradingCancelEvent");
            this._packetNames.Add(ClientPacketHeader.TradingConfirmMessageEvent, "TradingConfirmEvent");
            this._packetNames.Add(ClientPacketHeader.TradingModifyMessageEvent, "TradingModifyEvent");
            this._packetNames.Add(ClientPacketHeader.TradingCancelConfirmMessageEvent, "TradingCancelConfirmEvent");
            this._packetNames.Add(ClientPacketHeader.RequestFurniInventoryMessageEvent, "RequestFurniInventoryEvent");
            this._packetNames.Add(ClientPacketHeader.GetBadgesMessageEvent, "GetBadgesEvent");
            this._packetNames.Add(ClientPacketHeader.GetAchievementsMessageEvent, "GetAchievementsEvent");
            this._packetNames.Add(ClientPacketHeader.SetActivatedBadgesMessageEvent, "SetActivatedBadgesEvent");
            this._packetNames.Add(ClientPacketHeader.GetBotInventoryMessageEvent, "GetBotInventoryEvent");
            this._packetNames.Add(ClientPacketHeader.GetPetInventoryMessageEvent, "GetPetInventoryEvent");
            this._packetNames.Add(ClientPacketHeader.AvatarEffectActivatedMessageEvent, "AvatarEffectActivatedEvent");
            this._packetNames.Add(ClientPacketHeader.AvatarEffectSelectedMessageEvent, "AvatarEffectSelectedEvent");
            this._packetNames.Add(ClientPacketHeader.GetTalentTrackMessageEvent, "GetTalentTrackEvent");
            this._packetNames.Add(ClientPacketHeader.GetCreditsInfoMessageEvent, "GetCreditsInfoEvent");
            this._packetNames.Add(ClientPacketHeader.GetHabboClubWindowMessageEvent, "GetHabboClubWindowEvent");
            this._packetNames.Add(ClientPacketHeader.ScrGetUserInfoMessageEvent, "ScrGetUserInfoEvent");
            this._packetNames.Add(ClientPacketHeader.SetChatPreferenceMessageEvent, "SetChatPreferenceEvent");
            this._packetNames.Add(ClientPacketHeader.SetUserFocusPreferenceEvent, "SetUserFocusPreferenceEvent");
            this._packetNames.Add(ClientPacketHeader.SetMessengerInviteStatusMessageEvent, "SetMessengerInviteStatusEvent");
            this._packetNames.Add(ClientPacketHeader.RespectUserMessageEvent, "RespectUserEvent");
            this._packetNames.Add(ClientPacketHeader.UpdateFigureDataMessageEvent, "UpdateFigureDataEvent");
            this._packetNames.Add(ClientPacketHeader.OpenPlayerProfileMessageEvent, "OpenPlayerProfileEvent");
            this._packetNames.Add(ClientPacketHeader.GetSelectedBadgesMessageEvent, "GetSelectedBadgesEvent");
            this._packetNames.Add(ClientPacketHeader.GetRelationshipsMessageEvent, "GetRelationshipsEvent");
            this._packetNames.Add(ClientPacketHeader.SetRelationshipMessageEvent, "SetRelationshipEvent");
            this._packetNames.Add(ClientPacketHeader.CheckValidNameMessageEvent, "CheckValidNameEvent");
            this._packetNames.Add(ClientPacketHeader.ChangeNameMessageEvent, "ChangeNameEvent");
            this._packetNames.Add(ClientPacketHeader.GetHabboGroupBadgesMessageEvent, "GetHabboGroupBadgesEvent");
            this._packetNames.Add(ClientPacketHeader.GetUserTagsMessageEvent, "GetUserTagsEvent");
            this._packetNames.Add(ClientPacketHeader.SetSoundSettingsMessageEvent, "SetSoundSettingsEvent");
            this._packetNames.Add(ClientPacketHeader.GetSongInfoMessageEvent, "GetSongInfoEvent");
            this._packetNames.Add(ClientPacketHeader.EventTrackerMessageEvent, "EventTrackerEvent");
            this._packetNames.Add(ClientPacketHeader.ClientVariablesMessageEvent, "ClientVariablesEvent");
            this._packetNames.Add(ClientPacketHeader.DisconnectionMessageEvent, "DisconnectEvent");
            this._packetNames.Add(ClientPacketHeader.LatencyTestMessageEvent, "LatencyTestEvent");
            this._packetNames.Add(ClientPacketHeader.MemoryPerformanceMessageEvent, "MemoryPerformanceEvent");
            this._packetNames.Add(ClientPacketHeader.SetFriendBarStateMessageEvent, "SetFriendBarStateEvent");
            this._packetNames.Add(ClientPacketHeader.MessengerInitMessageEvent, "MessengerInitEvent");
            this._packetNames.Add(ClientPacketHeader.GetBuddyRequestsMessageEvent, "GetBuddyRequestsEvent");
            this._packetNames.Add(ClientPacketHeader.FollowFriendMessageEvent, "FollowFriendEvent");
            this._packetNames.Add(ClientPacketHeader.FindNewFriendsMessageEvent, "FindNewFriendsEvent");
            this._packetNames.Add(ClientPacketHeader.FriendListUpdateMessageEvent, "FriendListUpdateEvent");
            this._packetNames.Add(ClientPacketHeader.RemoveBuddyMessageEvent, "RemoveBuddyEvent");
            this._packetNames.Add(ClientPacketHeader.RequestBuddyMessageEvent, "RequestBuddyEvent");
            this._packetNames.Add(ClientPacketHeader.SendMsgMessageEvent, "SendMsgEvent");
            this._packetNames.Add(ClientPacketHeader.SendRoomInviteMessageEvent, "SendRoomInviteEvent");
            this._packetNames.Add(ClientPacketHeader.HabboSearchMessageEvent, "HabboSearchEvent");
            this._packetNames.Add(ClientPacketHeader.AcceptBuddyMessageEvent, "AcceptBuddyEvent");
            this._packetNames.Add(ClientPacketHeader.DeclineBuddyMessageEvent, "DeclineBuddyEvent");
            this._packetNames.Add(ClientPacketHeader.JoinGroupMessageEvent, "JoinGroupEvent");
            this._packetNames.Add(ClientPacketHeader.RemoveGroupFavouriteMessageEvent, "RemoveGroupFavouriteEvent");
            this._packetNames.Add(ClientPacketHeader.SetGroupFavouriteMessageEvent, "SetGroupFavouriteEvent");
            this._packetNames.Add(ClientPacketHeader.GetGroupInfoMessageEvent, "GetGroupInfoEvent");
            this._packetNames.Add(ClientPacketHeader.GetGroupMembersMessageEvent, "GetGroupMembersEvent");
            this._packetNames.Add(ClientPacketHeader.GetGroupCreationWindowMessageEvent, "GetGroupCreationWindowEvent");
            this._packetNames.Add(ClientPacketHeader.GetBadgeEditorPartsMessageEvent, "GetBadgeEditorPartsEvent");
            this._packetNames.Add(ClientPacketHeader.PurchaseGroupMessageEvent, "PurchaseGroupEvent");
            this._packetNames.Add(ClientPacketHeader.UpdateGroupIdentityMessageEvent, "UpdateGroupIdentityEvent");
            this._packetNames.Add(ClientPacketHeader.UpdateGroupBadgeMessageEvent, "UpdateGroupBadgeEvent");
            this._packetNames.Add(ClientPacketHeader.UpdateGroupColoursMessageEvent, "UpdateGroupColoursEvent");
            this._packetNames.Add(ClientPacketHeader.UpdateGroupSettingsMessageEvent, "UpdateGroupSettingsEvent");
            this._packetNames.Add(ClientPacketHeader.ManageGroupMessageEvent, "ManageGroupEvent");
            this._packetNames.Add(ClientPacketHeader.GiveAdminRightsMessageEvent, "GiveAdminRightsEvent");
            this._packetNames.Add(ClientPacketHeader.TakeAdminRightsMessageEvent, "TakeAdminRightsEvent");
            this._packetNames.Add(ClientPacketHeader.RemoveGroupMemberMessageEvent, "RemoveGroupMemberEvent");
            this._packetNames.Add(ClientPacketHeader.AcceptGroupMembershipMessageEvent, "AcceptGroupMembershipEvent");
            this._packetNames.Add(ClientPacketHeader.DeclineGroupMembershipMessageEvent, "DeclineGroupMembershipEvent");
            this._packetNames.Add(ClientPacketHeader.DeleteGroupMessageEvent, "DeleteGroupEvent");
            this._packetNames.Add(ClientPacketHeader.GetForumsListDataMessageEvent, "GetForumsListDataEvent");
            this._packetNames.Add(ClientPacketHeader.GetForumStatsMessageEvent, "GetForumStatsEvent");
            this._packetNames.Add(ClientPacketHeader.GetThreadsListDataMessageEvent, "GetThreadsListDataEvent");
            this._packetNames.Add(ClientPacketHeader.GetThreadDataMessageEvent, "GetThreadDataEvent");
            this._packetNames.Add(ClientPacketHeader.GetRoomSettingsMessageEvent, "GetRoomSettingsEvent");
            this._packetNames.Add(ClientPacketHeader.SaveRoomSettingsMessageEvent, "SaveRoomSettingsEvent");
            this._packetNames.Add(ClientPacketHeader.DeleteRoomMessageEvent, "DeleteRoomEvent");
            this._packetNames.Add(ClientPacketHeader.ToggleMuteToolMessageEvent, "ToggleMuteToolEvent");
            this._packetNames.Add(ClientPacketHeader.GetRoomFilterListMessageEvent, "GetRoomFilterListEvent");
            this._packetNames.Add(ClientPacketHeader.ModifyRoomFilterListMessageEvent, "ModifyRoomFilterListEvent");
            this._packetNames.Add(ClientPacketHeader.GetRoomRightsMessageEvent, "GetRoomRightsEvent");
            this._packetNames.Add(ClientPacketHeader.GetRoomBannedUsersMessageEvent, "GetRoomBannedUsersEvent");
            this._packetNames.Add(ClientPacketHeader.UnbanUserFromRoomMessageEvent, "UnbanUserFromRoomEvent");
            this._packetNames.Add(ClientPacketHeader.SaveEnforcedCategorySettingsMessageEvent, "SaveEnforcedCategorySettingsEvent");
            this._packetNames.Add(ClientPacketHeader.RespectPetMessageEvent, "RespectPetEvent");
            this._packetNames.Add(ClientPacketHeader.GetPetInformationMessageEvent, "GetPetInformationEvent");
            this._packetNames.Add(ClientPacketHeader.PickUpPetMessageEvent, "PickUpPetEvent");
            this._packetNames.Add(ClientPacketHeader.PlacePetMessageEvent, "PlacePetEvent");
            this._packetNames.Add(ClientPacketHeader.RideHorseMessageEvent, "RideHorseEvent");
            this._packetNames.Add(ClientPacketHeader.ApplyHorseEffectMessageEvent, "ApplyHorseEffectEvent");
            this._packetNames.Add(ClientPacketHeader.RemoveSaddleFromHorseMessageEvent, "RemoveSaddleFromHorseEvent");
            this._packetNames.Add(ClientPacketHeader.ModifyWhoCanRideHorseMessageEvent, "ModifyWhoCanRideHorseEvent");
            this._packetNames.Add(ClientPacketHeader.GetPetTrainingPanelMessageEvent, "GetPetTrainingPanelEvent");
            this._packetNames.Add(ClientPacketHeader.PlaceBotMessageEvent, "PlaceBotEvent");
            this._packetNames.Add(ClientPacketHeader.PickUpBotMessageEvent, "PickUpBotEvent");
            this._packetNames.Add(ClientPacketHeader.OpenBotActionMessageEvent, "OpenBotActionEvent");
            this._packetNames.Add(ClientPacketHeader.SaveBotActionMessageEvent, "SaveBotActionEvent");
            this._packetNames.Add(ClientPacketHeader.UpdateMagicTileMessageEvent, "UpdateMagicTileEvent");
            this._packetNames.Add(ClientPacketHeader.GetYouTubeTelevisionMessageEvent, "GetYouTubeTelevisionEvent");
            this._packetNames.Add(ClientPacketHeader.GetRentableSpaceMessageEvent, "GetRentableSpaceEvent");
            this._packetNames.Add(ClientPacketHeader.ToggleYouTubeVideoMessageEvent, "ToggleYouTubeVideoEvent");
            this._packetNames.Add(ClientPacketHeader.YouTubeVideoInformationMessageEvent, "YouTubeVideoInformationEvent");
            this._packetNames.Add(ClientPacketHeader.YouTubeGetNextVideo, "YouTubeGetNextVideo");
            this._packetNames.Add(ClientPacketHeader.SaveWiredTriggeRconfigMessageEvent, "SaveWiredConfigEvent");
            this._packetNames.Add(ClientPacketHeader.SaveWiredEffectConfigMessageEvent, "SaveWiredConfigEvent");
            this._packetNames.Add(ClientPacketHeader.SaveWiredConditionConfigMessageEvent, "SaveWiredConfigEvent");
            this._packetNames.Add(ClientPacketHeader.SaveBrandingItemMessageEvent, "SaveBrandingItemEvent");
            this._packetNames.Add(ClientPacketHeader.SetTonerMessageEvent, "SetTonerEvent");
            this._packetNames.Add(ClientPacketHeader.DiceOffMessageEvent, "DiceOffEvent");
            this._packetNames.Add(ClientPacketHeader.ThrowDiceMessageEvent, "ThrowDiceEvent");
            this._packetNames.Add(ClientPacketHeader.SetMannequinNameMessageEvent, "SetMannequinNameEvent");
            this._packetNames.Add(ClientPacketHeader.SetMannequinFigureMessageEvent, "SetMannequinFigureEvent");
            this._packetNames.Add(ClientPacketHeader.CreditFurniRedeemMessageEvent, "CreditFurniRedeemEvent");
            this._packetNames.Add(ClientPacketHeader.GetStickyNoteMessageEvent, "GetStickyNoteEvent");
            this._packetNames.Add(ClientPacketHeader.AddStickyNoteMessageEvent, "AddStickyNoteEvent");
            this._packetNames.Add(ClientPacketHeader.UpdateStickyNoteMessageEvent, "UpdateStickyNoteEvent");
            this._packetNames.Add(ClientPacketHeader.DeleteStickyNoteMessageEvent, "DeleteStickyNoteEvent");
            this._packetNames.Add(ClientPacketHeader.GetMoodlightConfigMessageEvent, "GetMoodlightConfigEvent");
            this._packetNames.Add(ClientPacketHeader.MoodlightUpdateMessageEvent, "MoodlightUpdateEvent");
            this._packetNames.Add(ClientPacketHeader.ToggleMoodlightMessageEvent, "ToggleMoodlightEvent");
            this._packetNames.Add(ClientPacketHeader.UseOneWayGateMessageEvent, "UseFurnitureEvent");
            this._packetNames.Add(ClientPacketHeader.UseHabboWheelMessageEvent, "UseFurnitureEvent");
            this._packetNames.Add(ClientPacketHeader.OpenGiftMessageEvent, "OpenGiftEvent");
            this._packetNames.Add(ClientPacketHeader.GetGroupFurniSettingsMessageEvent, "GetGroupFurniSettingsEvent");
            this._packetNames.Add(ClientPacketHeader.UseSellableClothingMessageEvent, "UseSellableClothingEvent");
            this._packetNames.Add(ClientPacketHeader.ConfirmLoveLockMessageEvent, "ConfirmLoveLockEvent");
            this._packetNames.Add(ClientPacketHeader.SaveFloorPlanModelMessageEvent, "SaveFloorPlanModelEvent");
            this._packetNames.Add(ClientPacketHeader.InitializeFloorPlanSessionMessageEvent, "InitializeFloorPlanSessionEvent");
            this._packetNames.Add(ClientPacketHeader.FloorPlanEditorRoomPropertiesMessageEvent, "FloorPlanEditorRoomPropertiesEvent");
            this._packetNames.Add(ClientPacketHeader.OpenHelpToolMessageEvent, "OpenHelpToolEvent");
            this._packetNames.Add(ClientPacketHeader.GetModeratorRoomInfoMessageEvent, "GetModeratorRoomInfoEvent");
            this._packetNames.Add(ClientPacketHeader.GetModeratorUserInfoMessageEvent, "GetModeratorUserInfoEvent");
            this._packetNames.Add(ClientPacketHeader.GetModeratorUserRoomVisitsMessageEvent, "GetModeratorUserRoomVisitsEvent");
            this._packetNames.Add(ClientPacketHeader.ModerateRoomMessageEvent, "ModerateRoomEvent");
            this._packetNames.Add(ClientPacketHeader.ModeratorActionMessageEvent, "ModeratorActionEvent");
            this._packetNames.Add(ClientPacketHeader.SubmitNewTicketMessageEvent, "SubmitNewTicketEvent");
            this._packetNames.Add(ClientPacketHeader.GetModeratorRoomChatlogMessageEvent, "GetModeratorRoomChatlogEvent");
            this._packetNames.Add(ClientPacketHeader.GetModeratorUserChatlogMessageEvent, "GetModeratorUserChatlogEvent");
            this._packetNames.Add(ClientPacketHeader.GetModeratorTicketChatlogsMessageEvent, "GetModeratorTicketChatlogsEvent");
            this._packetNames.Add(ClientPacketHeader.PickTicketMessageEvent, "PickTicketEvent");
            this._packetNames.Add(ClientPacketHeader.ReleaseTicketMessageEvent, "ReleaseTicketEvent");
            this._packetNames.Add(ClientPacketHeader.CloseTicketMesageEvent, "CloseTicketEvent");
            this._packetNames.Add(ClientPacketHeader.ModerationMuteMessageEvent, "ModerationMuteEvent");
            this._packetNames.Add(ClientPacketHeader.ModerationKickMessageEvent, "ModerationKickEvent");
            this._packetNames.Add(ClientPacketHeader.ModerationBanMessageEvent, "ModerationBanEvent");
            this._packetNames.Add(ClientPacketHeader.ModerationMsgMessageEvent, "ModerationMsgEvent");
            this._packetNames.Add(ClientPacketHeader.ModerationCautionMessageEvent, "ModerationCautionEvent");
            this._packetNames.Add(ClientPacketHeader.ModerationTradeLockMessageEvent, "ModerationTradeLockEvent");
            this._packetNames.Add(ClientPacketHeader.GetGameListingMessageEvent, "GetGameListingEvent");
            this._packetNames.Add(ClientPacketHeader.InitializeGameCenterMessageEvent, "InitializeGameCenterEvent");
            this._packetNames.Add(ClientPacketHeader.GetPlayableGamesMessageEvent, "GetPlayableGamesEvent");
            this._packetNames.Add(ClientPacketHeader.JoinPlayerQueueMessageEvent, "JoinPlayerQueueEvent");
            this._packetNames.Add(ClientPacketHeader.Game2GetWeeklyLeaderboardMessageEvent, "Game2GetWeeklyLeaderboardEvent");
        }
    }
}