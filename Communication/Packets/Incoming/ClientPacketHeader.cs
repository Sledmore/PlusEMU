namespace Plus.Communication.Packets.Incoming
{
    public static class ClientPacketHeader
    {
        // Handshake
        public const int InitCryptoMessageEvent = 3392;//316
        public const int GenerateSecretKeyMessageEvent = 3622;//3847
        public const int UniqueIDMessageEvent = 3521;//1471
        public const int SSOTicketMessageEvent = 1989;//1778
        public const int InfoRetrieveMessageEvent = 2629;//186

        // Avatar
        public const int GetWardrobeMessageEvent = 3901;//765
        public const int SaveWardrobeOutfitMessageEvent = 1777;//55

        // Catalog
        public const int GetCatalogIndexMessageEvent = 3226;//1294
        public const int GetCatalogPageMessageEvent = 60;//39
        public const int PurchaseFromCatalogMessageEvent = 3492;//2830
        public const int PurchaseFromCatalogAsGiftMessageEvent = 1555;//21

        // Navigator

        // Messenger
        public const int GetBuddyRequestsMessageEvent = 1646;//2485

        // Quests
        public const int GetQuestListMessageEvent = 2198;//2305        
        public const int StartQuestMessageEvent = 2457;//1282
        public const int GetCurrentQuestMessageEvent = 651;//90
        public const int CancelQuestMessageEvent = 104;//3879

        // Room Avatar
        public const int ActionMessageEvent = 3268;//3639
        public const int ApplySignMessageEvent = 3555;//2966
        public const int DanceMessageEvent = 1225;//645
        public const int SitMessageEvent = 3735;//1565
        public const int ChangeMottoMessageEvent = 674;//3515
        public const int LookToMessageEvent = 1142;//3744
        public const int DropHandItemMessageEvent = 3296;//1751

        // Room Connection
        public const int OpenFlatConnectionMessageEvent = 189;//407
        public const int GoToFlatMessageEvent = 2947;//1601

        // Room Chat
        public const int ChatMessageEvent = 744;//670
        public const int ShoutMessageEvent = 697;//2101
        public const int WhisperMessageEvent = 3003;//878

        // Room Engine

        // Room Furniture

        // Room Settings

        // Room Action

        // Users
        public const int GetIgnoredUsersMessageEvent = 198;

        // Moderation
        public const int OpenHelpToolMessageEvent = 1282;//1839
        public const int CallForHelpPendingCallsDeletedMessageEvent = 3643;
        public const int ModeratorActionMessageEvent = 760;//781
        public const int ModerationMsgMessageEvent = 2348;//2375        
        public const int ModerationMuteMessageEvent = 2474;//1940
        public const int ModerationTradeLockMessageEvent = 3955;//1160
        public const int GetModeratorUserRoomVisitsMessageEvent = 3848;//730
        public const int ModerationKickMessageEvent = 1011;//3589
        public const int GetModeratorRoomInfoMessageEvent = 1997;//182
        public const int GetModeratorUserInfoMessageEvent = 2677;//2984
        public const int GetModeratorRoomChatlogMessageEvent = 3216;//2312
        public const int ModerateRoomMessageEvent = 500;//3458
        public const int GetModeratorUserChatlogMessageEvent = 63;//695
        public const int GetModeratorTicketChatlogsMessageEvent = 1449;//3484
        public const int ModerationCautionMessageEvent = 2223;//505
        public const int ModerationBanMessageEvent = 2473;//2595
        public const int SubmitNewTicketMessageEvent = 1046;//963
        public const int CloseIssueDefaultActionEvent = 1921;

        // Inventory
        public const int GetCreditsInfoMessageEvent = 1051;//3697
        public const int GetAchievementsMessageEvent = 2249;//2931
        public const int GetBadgesMessageEvent = 2954;//166
        public const int RequestFurniInventoryMessageEvent = 2395;//352
        public const int SetActivatedBadgesMessageEvent = 2355;//2752
        public const int AvatarEffectActivatedMessageEvent = 2658;//129
        public const int AvatarEffectSelectedMessageEvent = 1816;//628

        public const int InitTradeMessageEvent = 3399;//3313
        public const int TradingCancelConfirmMessageEvent = 3738;//2264
        public const int TradingModifyMessageEvent = 644;//1153
        public const int TradingOfferItemMessageEvent = 842;//114
        public const int TradingCancelMessageEvent = 2934;//2967
        public const int TradingConfirmMessageEvent = 1394;//2399
        public const int TradingOfferItemsMessageEvent = 1607;//2996
        public const int TradingRemoveItemMessageEvent = 3313;//1033
        public const int TradingAcceptMessageEvent = 247;//3374

        // Register
        public const int UpdateFigureDataMessageEvent = 498;//2560

        // Groups
        public const int GetBadgeEditorPartsMessageEvent = 3706;//1670
        public const int GetGroupCreationWindowMessageEvent = 365;//468
        public const int GetGroupFurniSettingsMessageEvent = 1062;//41
        public const int DeclineGroupMembershipMessageEvent = 1571;//403
        public const int JoinGroupMessageEvent = 748;//2615
        public const int UpdateGroupColoursMessageEvent = 3469;//1443
        public const int SetGroupFavouriteMessageEvent = 77;//2625
        public const int GetGroupMembersMessageEvent = 3181;//205

        // Group Forums
        public const int PostGroupContentMessageEvent = 1499;//477
        public const int GetForumStatsMessageEvent = 1126;//872

        // Sound


        public const int RemoveMyRightsMessageEvent = 111;//879
        public const int GiveHandItemMessageEvent = 2523;//3315
        public const int GetClubGiftsMessageEvent = 3127;//3302
        public const int GoToHotelViewMessageEvent = 1429;//3576
        public const int GetRoomFilterListMessageEvent = 179;//1348
        public const int GetPromoArticlesMessageEvent = 2782;//3895
        public const int ModifyWhoCanRideHorseMessageEvent = 3604;//1993
        public const int RemoveBuddyMessageEvent = 1636;//698
        public const int RefreshCampaignMessageEvent = 3960;//3544
        public const int AcceptBuddyMessageEvent = 2067;//45
        public const int YouTubeVideoInformationMessageEvent = 1295;//2395
        public const int FollowFriendMessageEvent = 848;//2280
        public const int SaveBotActionMessageEvent = 2921;//678g
        public const int LetUserInMessageEvent = 1781;//2356
        public const int GetMarketplaceItemStatsMessageEvent = 1561;//1203
        public const int GetSellablePetBreedsMessageEvent = 599;//2505
        public const int ForceOpenCalendarBoxMessageEvent = 1275;//2879
        public const int SetFriendBarStateMessageEvent = 3841;//716
        public const int DeleteRoomMessageEvent = 439;//722
        public const int SetSoundSettingsMessageEvent = 608;//3820
        public const int InitializeGameCenterMessageEvent = 1825;//751
        public const int RedeemOfferCreditsMessageEvent = 2879;//1207
        public const int FriendListUpdateMessageEvent = 1166;//2664
        public const int ConfirmLoveLockMessageEvent = 3873;//2082
        public const int UseHabboWheelMessageEvent = 2148;//2651
        public const int SaveRoomSettingsMessageEvent = 3023;//2074
        public const int ToggleMoodlightMessageEvent = 14;//1826
        public const int GetDailyQuestMessageEvent = 3441;//484
        public const int SetMannequinNameMessageEvent = 3262;//2406
        public const int UseOneWayGateMessageEvent = 1970;//2816
        public const int EventTrackerMessageEvent = 143;//2386
        public const int FloorPlanEditorRoomPropertiesMessageEvent = 2478;//24
        public const int PickUpPetMessageEvent = 3975;//2342        
        public const int GetPetInventoryMessageEvent = 3646;//263
        public const int InitializeFloorPlanSessionMessageEvent = 3069;//2623
        public const int GetOwnOffersMessageEvent = 360;//3829
        public const int CheckPetNameMessageEvent = 3733;//159
        public const int SetUserFocusPreferenceEvent = 799;//526
        public const int SubmitBullyReportMessageEvent = 3971;//1803
        public const int RemoveRightsMessageEvent = 877;//40
        public const int MakeOfferMessageEvent = 2308;//255
        public const int KickUserMessageEvent = 1336;//3929
        public const int GetRoomSettingsMessageEvent = 581;//1014
        public const int GetThreadsListDataMessageEvent = 2568;//1606
        public const int GetForumUserProfileMessageEvent = 3515;//2639
        public const int SaveWiredEffectConfigMessageEvent = 2234;//3431
        public const int GetRoomEntryDataMessageEvent = 1747;//2768
        public const int JoinPlayerQueueMessageEvent = 167;//951
        public const int CanCreateRoomMessageEvent = 2411;//361
        public const int SetTonerMessageEvent = 1389;//1061
        public const int SaveWiredTriggeRconfigMessageEvent = 3877;//1897
        public const int PlaceBotMessageEvent = 3770;//2321
        public const int GetRelationshipsMessageEvent = 3046;//866
        public const int SetMessengerInviteStatusMessageEvent = 1663;//1379
        public const int UseFurnitureMessageEvent = 3249;//3846
        public const int GetUserFlatCatsMessageEvent = 493;//3672
        public const int AssignRightsMessageEvent = 3843;//3574
        public const int GetRoomBannedUsersMessageEvent = 2009;//581
        public const int ReleaseTicketMessageEvent = 3931;//3800
        public const int OpenPlayerProfileMessageEvent = 3053;//3591
        public const int GetSanctionStatusMessageEvent = 3209;//2883
        public const int CreditFurniRedeemMessageEvent = 3945;//1676
        public const int DisconnectionMessageEvent = 1474;//2391
        public const int PickupObjectMessageEvent = 1766;//636
        public const int FindRandomFriendingRoomMessageEvent = 2189;//1874
        public const int UseSellableClothingMessageEvent = 2849;//818
        public const int MoveObjectMessageEvent = 3583;//1781
        public const int GetFurnitureAliasesMessageEvent = 3116;//2125
        public const int TakeAdminRightsMessageEvent = 1661;//2725
        public const int ModifyRoomFilterListMessageEvent = 87;//256
        public const int MoodlightUpdateMessageEvent = 2913;//856
        public const int GetPetTrainingPanelMessageEvent = 3915;//2088
        public const int GetSongInfoMessageEvent = 3916;//3418
        public const int UseWallItemMessageEvent = 3674;//3396
        public const int GetTalentTrackMessageEvent = 680;//1284
        public const int GiveAdminRightsMessageEvent = 404;//465
        public const int GetCatalogModeMessageEvent = 951;//2267
        public const int SendBullyReportMessageEvent = 3540;//2973
        public const int CancelOfferMessageEvent = 195;//1862
        public const int SaveWiredConditionConfigMessageEvent = 2370;//488
        public const int RedeemVoucherMessageEvent = 1384;//489
        public const int ThrowDiceMessageEvent = 3427;//1182
        public const int CraftSecretMessageEvent = 3623;//1622
        public const int GetGameListingMessageEvent = 705;//2993
        public const int SetRelationshipMessageEvent = 1514;//2112
        public const int RequestBuddyMessageEvent = 1706;//3775
        public const int MemoryPerformanceMessageEvent = 124;//731
        public const int ToggleYouTubeVideoMessageEvent = 1956;//890
        public const int SetMannequinFigureMessageEvent = 1909;//3936
        public const int GetEventCategoriesMessageEvent = 597;//1086
        public const int DeleteGroupThreadMessageEvent = 50;//3299
        public const int PurchaseGroupMessageEvent = 2959;//2546
        public const int MessengerInitMessageEvent = 2825;//2151
        public const int CancelTypingMessageEvent = 1329;//1114
        public const int GetMoodlightConfigMessageEvent = 2906;//3472
        public const int GetGroupInfoMessageEvent = 681;//3211
        public const int CreateFlatMessageEvent = 92;//3077
        public const int LatencyTestMessageEvent = 878;//1789
        public const int GetSelectedBadgesMessageEvent = 2735;//2226
        public const int AddStickyNoteMessageEvent = 3891;//425
        public const int ChangeNameMessageEvent = 2709;//1067
        public const int RideHorseMessageEvent = 3387;//1440
        public const int InitializeNewNavigatorMessageEvent = 3375;//882
        public const int SetChatPreferenceMessageEvent = 1045;//2006
        public const int GetForumsListDataMessageEvent = 3802;//3912
        public const int ToggleMuteToolMessageEvent = 1301;//2462
        public const int UpdateGroupIdentityMessageEvent = 1375;//1062
        public const int UpdateStickyNoteMessageEvent = 3120;//342
        public const int UnbanUserFromRoomMessageEvent = 2050;//3060
        public const int UnIgnoreUserMessageEvent = 981;//3023
        public const int OpenGiftMessageEvent = 349;//1515
        public const int ApplyDecorationMessageEvent = 2729;//728
        public const int GetRecipeConfigMessageEvent = 2428;//3654
        public const int ScrGetUserInfoMessageEvent = 2749;//12
        public const int RemoveGroupMemberMessageEvent = 1590;//649
        public const int DiceOffMessageEvent = 1124;//191
        public const int YouTubeGetNextVideo = 2618;//1843
        public const int DeleteFavouriteRoomMessageEvent = 3223;//855
        public const int RespectUserMessageEvent = 3812;//1955
        public const int AddFavouriteRoomMessageEvent = 3251;//3092
        public const int DeclineBuddyMessageEvent = 3484;//835
        public const int StartTypingMessageEvent = 2826;//3362
        public const int GetGroupFurniConfigMessageEvent = 3902;//3046
        public const int SendRoomInviteMessageEvent = 1806;//2694
        public const int RemoveAllRightsMessageEvent = 884;//1404
        public const int GetYouTubeTelevisionMessageEvent = 1326;//3517
        public const int FindNewFriendsMessageEvent = 3889;//1264
        public const int GetPromotableRoomsMessageEvent = 2306;//276
        public const int GetBotInventoryMessageEvent = 775;//363
        public const int GetRentableSpaceMessageEvent = 2035;//793
        public const int OpenBotActionMessageEvent = 3236;//2544
        public const int OpenCalendarBoxMessageEvent = 1229;//724
        public const int DeleteGroupPostMessageEvent = 1991;//317
        public const int CheckValidNameMessageEvent = 2507;//8
        public const int UpdateGroupBadgeMessageEvent = 1589;//2959
        public const int PlaceObjectMessageEvent = 1809;//579
        public const int RemoveGroupFavouriteMessageEvent = 226;//1412
        public const int UpdateNavigatorSettingsMessageEvent = 1824;//2501
        public const int CheckGnomeNameMessageEvent = 1179;//2281
        public const int NavigatorSearchMessageEvent = 618;//2722
        public const int GetPetInformationMessageEvent = 2986;//2853
        public const int GetGuestRoomMessageEvent = 2247;//1164
        public const int UpdateThreadMessageEvent = 2980;//1522
        public const int AcceptGroupMembershipMessageEvent = 2996;//2259
        public const int GetMarketplaceConfigurationMessageEvent = 2811;//1604
        public const int Game2GetWeeklyLeaderboardMessageEvent = 285;//2106
        public const int BuyOfferMessageEvent = 904;//3699
        public const int RemoveSaddleFromHorseMessageEvent = 844;//1892
        public const int GiveRoomScoreMessageEvent = 3261;//336
        public const int GetHabboClubWindowMessageEvent = 3530;//715
        public const int DeleteStickyNoteMessageEvent = 3885;//2777
        public const int MuteUserMessageEvent = 2101;//2997
        public const int ApplyHorseEffectMessageEvent = 3364;//870
        public const int GetClientVersionMessageEvent = 4000;//4000
        public const int OnBullyClickMessageEvent = 254;//1932
        public const int HabboSearchMessageEvent = 1194;//3375
        public const int PickTicketMessageEvent = 1807;//3973
        public const int GetGiftWrappingConfigurationMessageEvent = 1570;//1928
        public const int GetCraftingRecipesAvailableMessageEvent = 1869;//1653
        public const int GetThreadDataMessageEvent = 2324;//1559
        public const int ManageGroupMessageEvent = 737;//2547
        public const int PlacePetMessageEvent = 1495;//223
        public const int EditRoomPromotionMessageEvent = 816;//3707
        public const int GetCatalogOfferMessageEvent = 362;//2180
        public const int SaveFloorPlanModelMessageEvent = 1936;//1287
        public const int MoveWallItemMessageEvent = 1778;//609
        public const int ClientVariablesMessageEvent = 1220;//1600
        public const int PingMessageEvent = 509;//2584
        public const int DeleteGroupMessageEvent = 114;//747
        public const int UpdateGroupSettingsMessageEvent = 2435;//3180
        public const int GetRecyclerRewardsMessageEvent = 2152;//3258
        public const int PurchaseRoomPromotionMessageEvent = 1542;//3078
        public const int PickUpBotMessageEvent = 3058;//644
        public const int GetOffersMessageEvent = 2817;//442
        public const int GetHabboGroupBadgesMessageEvent = 3925;//301
        public const int GetUserTagsMessageEvent = 84;//1722
        public const int GetPlayableGamesMessageEvent = 1418;//482
        public const int GetCatalogRoomPromotionMessageEvent = 2757;//538
        public const int MoveAvatarMessageEvent = 2121;//1737
        public const int SaveBrandingItemMessageEvent = 2208;//3156
        public const int SaveEnforcedCategorySettingsMessageEvent = 531;//3413
        public const int RespectPetMessageEvent = 1967;//1618
        public const int GetMarketplaceCanMakeOfferMessageEvent = 1552;//1647
        public const int UpdateMagicTileMessageEvent = 2997;//1248
        public const int GetStickyNoteMessageEvent = 2469;//2796
        public const int IgnoreUserMessageEvent = 2374;//2394
        public const int BanUserMessageEvent = 3009;//3940
        public const int UpdateForumSettingsMessageEvent = 3295;//931
        public const int GetRoomRightsMessageEvent = 3937;//2734
        public const int SendMsgMessageEvent = 2409;//1981
        public const int CloseTicketMesageEvent = 1080;//50
    }
}