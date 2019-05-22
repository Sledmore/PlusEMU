namespace Plus.Communication.Packets.Incoming
{
    public static class ClientPacketHeader
    {
        // Handshake
        public const int InitCryptoMessageEvent = 2374;//3392;
        public const int GenerateSecretKeyMessageEvent = 3823;//3622
        public const int UniqueIDMessageEvent = 2701;//3521
        public const int SSOTicketMessageEvent = 1029;//1989
        public const int InfoRetrieveMessageEvent = 2078;//2629
                                                        
        //Ambassador
        public const int AmbassadorAlertMessageEvent = 2549; //done
        public const int HandleHelperToolMessageEvent = 540; //done
        public const int AceptJoinJudgeChatMessageEvent = 65;//done
        public const int CancelCallForHelperMessageEvent = 25;//done
        public const int CallForHelperMessageEvent = 1581;//done
        public const int AcceptHelperSessionMessageEvent = 351;//done
        public const int HelperSessioChatTypingMessageEvent = 1777;//done
        public const int HelperSessioChatSendMessageMessageEvent = 1307;//done
        public const int CloseHelperChatSessionMessageEvent = 1136;//done
        public const int VisitHelperUserSessionMessageEvent = 2;//done
        public const int InvinteHelperUserSessionMessageEvent = 1305;//done
        public const int FinishHelperSessionMessageEvent = 1520;//done
        public const int ReportHelperSessionMessageEvent = 2038;//done

        //NUX
        public const int GetNuxPresentEvent = 122223;
        public const int NuxUnderstoodEvent = 863;

        //POLLS
        public const int AnswerPollQuestionMessageEvent = 1723; //OR 543
        public const int AcceptPollQuestionsMessageEvent = 2702; // or 281 //NEEDS TEST
        public const int DeclinePollQuestionsMessageEvent = 157;

        public const int GetCraftingItemMessageEvent = 1018; //new
        public const int SetCraftingRecipeMessageEvent = 2336;
        public const int ExecuteCraftingRecipeMessageEvent = 2806;//new

        public const int AddRoomToStaffPicksEvent = 1292;


        // Avatar
        public const int GetWardrobeMessageEvent = 1602;//3901
        public const int SaveWardrobeOutfitMessageEvent = 1794;//1777

        // Catalog
        public const int GetCatalogIndexMessageEvent = 1215;//3226
        public const int GetCatalogPageMessageEvent = 3365;//60
        public const int PurchaseFromCatalogMessageEvent = 2223;//3492
        public const int PurchaseFromCatalogAsGiftMessageEvent = 53;//1555

        // Navigator

        // Messenger
        public const int GetBuddyRequestsMessageEvent = 688;//1646

        // Quests
        public const int GetQuestListMessageEvent = 2081;//2198
        public const int StartQuestMessageEvent = 1395;//2457
        public const int GetCurrentQuestMessageEvent = 1107;//651
        public const int CancelQuestMessageEvent = 1985;//104

        // Room Avatar
        public const int ActionMessageEvent = 3097;//3268
        public const int ApplySignMessageEvent = 205;//3555
        public const int DanceMessageEvent = 1197;//1225
        public const int SitMessageEvent = 639;//3735
        public const int ChangeMottoMessageEvent = 570;//674
        public const int LookToMessageEvent = 1772;//1142
        public const int DropHandItemMessageEvent = 2776;//3296

        // Room Connection
        public const int OpenFlatConnectionMessageEvent = 3305;//189
        public const int GoToFlatMessageEvent = 982;//2947

        // Room Chat
        public const int ChatMessageEvent = 563;//744
        public const int ShoutMessageEvent = 1565;//697
        public const int WhisperMessageEvent = 2599;//3003

        // Room Engine

        // Room Furniture

        // Room Settings

        // Room Action

        // Users
        public const int GetIgnoredUsersMessageEvent = 2645;//198

        // Moderation
        public const int OpenHelpToolMessageEvent = 1781;//1282
        public const int CallForHelpPendingCallsDeletedMessageEvent = 1059;//3643
        public const int ModeratorActionMessageEvent = 3514;//760
        public const int ModerationMsgMessageEvent = 318;//2348
        public const int ModerationMuteMessageEvent = 508;//2474
        public const int ModerationTradeLockMessageEvent = 279;//3955
        public const int GetModeratorUserRoomVisitsMessageEvent = 2798;
        public const int ModerationKickMessageEvent = 1867;//1011
        public const int GetModeratorRoomInfoMessageEvent = 826;//1997
        public const int GetModeratorUserInfoMessageEvent = 1844;//2677
        public const int GetModeratorRoomChatlogMessageEvent = 906;//3216
        public const int ModerateRoomMessageEvent = 801;//500
        public const int GetModeratorUserChatlogMessageEvent = 2105;//63
        public const int GetModeratorTicketChatlogsMessageEvent = 450;//1449
        public const int ModerationCautionMessageEvent = 3844;//2223
        public const int ModerationBanMessageEvent = 3344;//2473
        public const int SubmitNewTicketMessageEvent = 2244;//1046
        public const int CloseIssueDefaultActionEvent = 682;//1921

        // Inventory
        public const int GetCreditsInfoMessageEvent = 2522;//1051
        public const int GetAchievementsMessageEvent = 1797;//2249
        public const int GetBadgesMessageEvent = 166;//2954
        public const int RequestFurniInventoryMessageEvent = 3818;//2395
        public const int SetActivatedBadgesMessageEvent = 2466;//2355
        public const int AvatarEffectActivatedMessageEvent = 3786;//2658
        public const int AvatarEffectSelectedMessageEvent = 1364;//1816

        public const int InitTradeMessageEvent = 293;//3399
        public const int TradingCancelConfirmMessageEvent = 1065;//3738
        public const int TradingModifyMessageEvent = 739;//644
        public const int TradingOfferItemMessageEvent = 2886;//842
        public const int TradingCancelMessageEvent = 1569;//2934
        public const int TradingConfirmMessageEvent = 2598;//1394
        public const int TradingOfferItemsMessageEvent = 1160;//1607
        public const int TradingRemoveItemMessageEvent = 1846;//3313
        public const int TradingAcceptMessageEvent = 1129;//247

        // Register
        public const int UpdateFigureDataMessageEvent = 1631;

        // Groups
        public const int GetBadgeEditorPartsMessageEvent = 121;//3706
        public const int GetGroupCreationWindowMessageEvent = 1051;//365
        public const int GetGroupFurniSettingsMessageEvent = 1786;//1062
        public const int DeclineGroupMembershipMessageEvent = 1308;//1571
        public const int JoinGroupMessageEvent = 3749;//748
        public const int UpdateGroupColoursMessageEvent = 1475;//3469
        public const int SetGroupFavouriteMessageEvent = 1604;//77
        public const int GetGroupMembersMessageEvent = 139;//3181

        // Group Forums
        public const int PostGroupContentMessageEvent = 794;//1499
        public const int GetForumStatsMessageEvent = 228;//1126
        public const int UpdateForumReadMarkerMessageEvent = 972;//1659
        public const int UpdateForumThreadStatusMessageEvent = 3724;//2980

        //Builders Club Packets xDDDD


        //Camera
        public const int HabboCameraEvent = -3221;
        public const int GetCameraRequestEvent = -1405;
        public const int HabboCameraPictureDataMessageEvent = -1405;
        public const int PublishCameraPictureMessageEvent = -2933;
        public const int PurchaseCameraPictureMessageEvent = -3221;
        public const int ParticipatePictureCameraCompetitionMessageEvent = -1419;

        // Sound


        public const int RemoveMyRightsMessageEvent = 673;//111
        public const int GiveHandItemMessageEvent = 467;//2523
        public const int GetClubGiftsMessageEvent = 3142;//3127
        public const int GetClubOffersMessageEvent = 13131331;//findme
        public const int GoToHotelViewMessageEvent = 2539;//1429
        public const int GetRoomFilterListMessageEvent = 566;//179
        public const int GetPromoArticlesMessageEvent = 3678;//2782
        public const int ModifyWhoCanRideHorseMessageEvent = 2253;//3604
        public const int RemoveBuddyMessageEvent = 3851;//1636
        public const int RefreshCampaignMessageEvent = 3134;//3960
        public const int AcceptBuddyMessageEvent = 408;//2067
        public const int YouTubeVideoInformationMessageEvent = 2294;//1295
        public const int FollowFriendMessageEvent = 659;//848
        public const int SaveBotActionMessageEvent = 909;//2921
        public const int LetUserInMessageEvent = 1670;//1781
        public const int GetMarketplaceItemStatsMessageEvent = 730;//1561
        public const int GetSellablePetBreedsMessageEvent = 3692;//599
        public const int ForceOpenCalendarBoxMessageEvent = 1405;//1275
        public const int SetFriendBarStateMessageEvent = 2932;//3841
        public const int DeleteRoomMessageEvent = 2990;//439
        public const int SetSoundSettingsMessageEvent = 3056;//608
        public const int InitializeGameCenterMessageEvent = 2594;//1825
        public const int RedeemOfferCreditsMessageEvent = 119;//2879
        public const int FriendListUpdateMessageEvent = 227;//1166
        public const int ConfirmLoveLockMessageEvent = 2019;//3873
        public const int UseHabboWheelMessageEvent = 1537;//2148
        public const int SaveRoomSettingsMessageEvent = 1099;//3023
        public const int ToggleMoodlightMessageEvent = 281;//14
        public const int GetDailyQuestMessageEvent = 2154;//3441
        public const int SetMannequinNameMessageEvent = 1055;//3262
        public const int UseOneWayGateMessageEvent = 2838;//1970
        public const int EventTrackerMessageEvent = 734;//143
        public const int FloorPlanEditorRoomPropertiesMessageEvent = 2796;//2478
        public const int PickUpPetMessageEvent = 2681;//3975
        public const int GetPetInventoryMessageEvent = 3735;//3646
        public const int InitializeFloorPlanSessionMessageEvent = 965;//3069
        public const int GetOwnOffersMessageEvent = 769;//360
        public const int CheckPetNameMessageEvent = 2794;//3733
        public const int SetUserFocusPreferenceEvent = 3405;//799
        public const int SubmitBullyReportMessageEvent = 3173;//3971
        public const int RemoveRightsMessageEvent = 1109;//877
        public const int MakeOfferMessageEvent = 1744;//2308
        public const int KickUserMessageEvent = 2301;//1336
        public const int GetRoomSettingsMessageEvent = 146;//581
        public const int GetThreadsListDataMessageEvent = 1148;//2568
        public const int GetForumUserProfileMessageEvent = 3959;//3515
        public const int SaveWiredEffectConfigMessageEvent = 513;//2234
        public const int GetRoomEntryDataMessageEvent = 1545;//1747
        public const int JoinPlayerQueueMessageEvent = 1357;//167
        public const int CanCreateRoomMessageEvent = 3614;//2411
        public const int SetTonerMessageEvent = 2931;//1389
        public const int SaveWiredTriggerConfigMessageEvent = 3892;//3877
        public const int PlaceBotMessageEvent = 7;//3770
        public const int GetRelationshipsMessageEvent = 155;//3046
        public const int SetMessengerInviteStatusMessageEvent = 3436;//1663
        public const int UseFurnitureMessageEvent = 926;//3249
        public const int GetUserFlatCatsMessageEvent = 3329;//493
        public const int AssignRightsMessageEvent = 948;//3843
        public const int GetRoomBannedUsersMessageEvent = 2359;//2009
        public const int ReleaseTicketMessageEvent = 2507;//3931
        public const int OpenPlayerProfileMessageEvent = 1058;
        public const int GetSanctionStatusMessageEvent = 1015;//3209
        public const int CreditFurniRedeemMessageEvent = 153;//3945
        public const int DisconnectionMessageEvent = 2057;//1474
        public const int PickupObjectMessageEvent = 1046;//1766
        public const int FindRandomFriendingRoomMessageEvent = 2638;//2189
        public const int UseSellableClothingMessageEvent = 3114;//2849
        public const int MoveObjectMessageEvent = 3174;//3583
        public const int GetFurnitureAliasesMessageEvent = 723;//3116
        public const int TakeAdminRightsMessageEvent = 258;//1661
        public const int ModifyRoomFilterListMessageEvent = 590;//87
        public const int MoodlightUpdateMessageEvent = 1203;//2913
        public const int GetPetTrainingPanelMessageEvent = 3907;//3915
        public const int GetSongInfoMessageEvent = 1511;//3916
        public const int UseWallItemMessageEvent = 264;//3674
        public const int GetTalentTrackMessageEvent = 3202;//680
        public const int CompleteSafetyQuiz = 1920;
        public const int GiveAdminRightsMessageEvent = 3116;//404
        public const int GetCatalogModeMessageEvent = 2481;//951
        public const int SendBullyReportMessageEvent = 1435;//3540
        public const int CancelOfferMessageEvent = 2913;//195
        public const int SaveWiredConditionConfigMessageEvent = 1820;//2370
        public const int RedeemVoucherMessageEvent = 3444;//1384
        public const int ThrowDiceMessageEvent = 2977;//3427
        public const int CraftSecretMessageEvent = 110;//3623
        public const int GetGameListingMessageEvent = 2056;//705
        public const int SetRelationshipMessageEvent = 930;//1514
        public const int RequestBuddyMessageEvent = 3816;//1706
        public const int MemoryPerformanceMessageEvent = 661;//124
        public const int ToggleYouTubeVideoMessageEvent = 2880;//1956
        public const int SetMannequinFigureMessageEvent = 1599;//1909
        public const int GetEventCategoriesMessageEvent = 3524;//597
        public const int DeleteGroupThreadMessageEvent = 3609;//50
        public const int PurchaseGroupMessageEvent = 1753;//2959
        public const int MessengerInitMessageEvent = 743;//2825
        public const int CancelTypingMessageEvent = 1986;//1329
        public const int GetMoodlightConfigMessageEvent = 1322;//2906
        public const int GetGroupInfoMessageEvent = 283;//681
        public const int CreateFlatMessageEvent = 3516;//92
        public const int LatencyTestMessageEvent = 2998;//878
        public const int GetSelectedBadgesMessageEvent = 1935;//2735
        public const int AddStickyNoteMessageEvent = 577;//3891
        public const int ChangeNameMessageEvent = 1834;//2709
        public const int RideHorseMessageEvent = 1481;//3387
        public const int InitializeNewNavigatorMessageEvent = 1217;//3375
        public const int SetChatPreferenceMessageEvent = 3582;//1045
        public const int GetForumsListDataMessageEvent = 918;//3802
        public const int ToggleMuteToolMessageEvent = 2677;//1301
        public const int UpdateGroupIdentityMessageEvent = 516;//1375
        public const int UpdateStickyNoteMessageEvent = 1847;//3120
        public const int UnbanUserFromRoomMessageEvent = 2700;//2050
        public const int UnIgnoreUserMessageEvent = 3677;//981
        public const int OpenGiftMessageEvent = 3867;//349
        public const int ApplyDecorationMessageEvent = 1416;//2729
        public const int GetRecipeConfigMessageEvent = 2336;//2428
        public const int ScrGetUserInfoMessageEvent = 220;//2749
        public const int RemoveGroupMemberMessageEvent = 2240;//1590
        public const int DiceOffMessageEvent = 1838;//1124
        public const int YouTubeGetNextVideo = 3788;//2618
        public const int DeleteFavouriteRoomMessageEvent = 3544;//3223
        public const int RespectUserMessageEvent = 3537;//3812
        public const int AddFavouriteRoomMessageEvent = 1413;//3251
        public const int DeclineBuddyMessageEvent = 3726;//3484
        public const int StartTypingMessageEvent = 403;//2826
        public const int GetGroupFurniConfigMessageEvent = 75;//3902
        public const int SendRoomInviteMessageEvent = 3746;//1806
        public const int RemoveAllRightsMessageEvent = 3296;//884
        public const int GetYouTubeTelevisionMessageEvent = 66;//1326
        public const int FindNewFriendsMessageEvent = 2653;//3889
        public const int GetPromotableRoomsMessageEvent = 2016;//2306
        public const int GetBotInventoryMessageEvent = 2017;//775
        public const int GetRentableSpaceMessageEvent = 2908;//2035
        public const int OpenBotActionMessageEvent = 836;//3236
        public const int OpenCalendarBoxMessageEvent = 634;//1229
        public const int DeleteGroupPostMessageEvent = 2519;//1991
        public const int CheckValidNameMessageEvent = 3014;//2507
        public const int UpdateGroupBadgeMessageEvent = 1082;//1589
        public const int PlaceObjectMessageEvent = 3651;//1809
        public const int RemoveGroupFavouriteMessageEvent = 2093;//226
        public const int UpdateNavigatorSettingsMessageEvent = 1738;//1824
        public const int CheckGnomeNameMessageEvent = 2325;//1179
        public const int NavigatorSearchMessageEvent = 2456;//618
        public const int GetPetInformationMessageEvent = 2139;//2986
        public const int GetGuestRoomMessageEvent = 2420;//2247
        public const int UpdateThreadMessageEvent = 3724;//2980
        public const int AcceptGroupMembershipMessageEvent = 3136;//2996
        public const int GetMarketplaceConfigurationMessageEvent = 3065;//2811
        public const int Game2GetWeeklyLeaderboardMessageEvent = 2929;//285
        public const int BuyOfferMessageEvent = 1677;//904
        public const int RemoveSaddleFromHorseMessageEvent = 994;//844
        public const int GiveRoomScoreMessageEvent = 3777;//3261
        public const int GetHabboClubWindowMessageEvent = 3031;//3530

        public const int DeleteStickyNoteMessageEvent = 2458;//3885
        public const int MuteUserMessageEvent = 2646;//2101
        public const int ApplyHorseEffectMessageEvent = 2262;//3364
        public const int GetClientVersionMessageEvent = 4000;//4000
        public const int OnBullyClickMessageEvent = 3953;//254
        public const int HabboSearchMessageEvent = 2745;//1194
        public const int PickTicketMessageEvent = 316;//1807
        public const int GetGiftWrappingConfigurationMessageEvent = 1027;//1570
        public const int GetCraftingRecipesAvailableMessageEvent = 1767;//1869
        public const int GetThreadDataMessageEvent = 2377;//2324
        public const int ManageGroupMessageEvent = 266;//737
        public const int PlacePetMessageEvent = 886;//1495
        public const int EditRoomPromotionMessageEvent = 2562;//816
        public const int GetCatalogOfferMessageEvent = 2907;//362
        public const int SaveFloorPlanModelMessageEvent = 1707;//1936
        public const int MoveWallItemMessageEvent = 663;//1778
        public const int ClientVariablesMessageEvent = 3126;//1220
        public const int PingMessageEvent = 1623;//509
        public const int DeleteGroupMessageEvent = 3320;//114
        public const int UpdateGroupSettingsMessageEvent = 2178;//2435
        public const int GetRecyclerRewardsMessageEvent = 1430;//2152
        public const int PurchaseRoomPromotionMessageEvent = 1839;//1542
        public const int PickUpBotMessageEvent = 2090;
        public const int GetOffersMessageEvent = 776;//2817
        public const int GetHabboGroupBadgesMessageEvent = 3020;//3925
        public const int GetUserTagsMessageEvent = 3200;//84
        public const int GetPlayableGamesMessageEvent = 2792;//1418
        public const int GetCatalogRoomPromotionMessageEvent = 891;//2757
        public const int MoveAvatarMessageEvent = 2923;//2121
        public const int SaveBrandingItemMessageEvent = 2926;//2208
        public const int SaveEnforcedCategorySettingsMessageEvent = 642;//531
        public const int RespectPetMessageEvent = 3804;//1967
        public const int GetMarketplaceCanMakeOfferMessageEvent = 3547;//1552
        public const int UpdateMagicTileMessageEvent = 1513;//2997
        public const int GetStickyNoteMessageEvent = 3389;//2469
        public const int IgnoreUserMessageEvent = 1473;//2374
        public const int BanUserMessageEvent = 464;//3009
        public const int UpdateForumSettingsMessageEvent = 2752;//3295
        public const int GetRoomRightsMessageEvent = 2772;//3937
        public const int SendMsgMessageEvent = 2083;//2409
        public const int CloseTicketMesageEvent = 3520;//1080
    }
}