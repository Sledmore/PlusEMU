namespace Plus.Communication.Packets.Outgoing
{
    public static class ServerPacketHeader
    {
        // Handshake 
        public const int InitCryptoMessageComposer = 1233;//3531
        public const int SecretKeyMessageComposer = 1631;//696
        public const int AuthenticationOKMessageComposer = 1294;//1079
        public const int UserObjectMessageComposer = 3231;//845
        public const int UserPerksMessageComposer = 3877;//1790
        public const int UserRightsMessageComposer = 975;//3315
        public const int GenericErrorMessageComposer = 1329;//905
        public const int SetUniqueIdMessageComposer = 226;//3731
        public const int AvailabilityStatusMessageComposer = 1312;//3690

        public const int SetJukeboxSongMusicDataMessageComposer = 715; //todo
        public const int SetJukeboxPlayListMessageComposer = 565;  //todo
        public const int SetJukeboxNowPlayingMessageComposer = 2272;  //todo
        public const int LoadJukeboxUserMusicItemsMessageComposer = 3402;  //todo

        //HELPER TOOL
        public const int HandleHelperToolMessageComposer = 3757;//done
        public const int CloseHelperSessionMessageComposer = 2116;//done
        public const int InitHelperSessionChatMessageComposer = 3601;//done
        public const int EndHelperSessionMessageComposer = 2747;//done
        public const int HelperSessionSendChatMessageComposer = 941;//done
        public const int HelperSessionVisiteRoomMessageComposer = 1338;//done
        public const int HelperSessionInvinteRoomMessageComposer = 3267;//done
        public const int HelperSessionChatIsTypingMessageComposer = 3523;//done
        public const int CallForHelperWindowMessageComposer = 3093;//done
        public const int CallForHelperErrorMessageComposer = 2913;//done

        //POLLS
        public const int SendPollInvinteMessageComposer = 2350;
        public const int PollQuestionsMessageComposer = 813;
        public const int PollErrorAlertMessageComposer = 1879;

        //new notif
        // public const int MassEventComposer = 2954;
        public const int HCGiftsAlertComposer = 919;

        //MEGA OFFER
        public const int openBoxTargetedOffert = 3209;

        //Camera
        public const int CameraSendImageUrlMessageComposer = 311; //completed 
        public const int CamereFinishPurchaseMessageComposer = 1103; //completed 
        public const int CameraFinishPublishMessageComposer = 2867; //completed 
        public const int CameraFinishParticipateCompetitionMessageComposer = 3553; //completed
        public const int SetCameraPicturePriceMessageComposer = 388; //completed
        public const int SendRoomThumbnailAlertMessageComposer = 2664; //completed

        //nux text
        public const int NuxSuggestFreeGiftsMessageComposer = 2433;
        public const int NuxItemListComposer = 3247;

        //QuickPoll
        public const int MatchingPollResultMessageComposer = 1087;
        public const int QuickPollResultMessageComposer = 692;

        //furni matic
        public const int FurniMaticNoRoomError = 2786;
        public const int FurniMaticReceiveItem = 249;
        public const int FurniMaticRewardsComposer = 1026;

        //HC TEST
        //public const int GetClubComposer = 8313;//863; //480;
        public const int ClubStatusMessageComposer = 2416;
        public const int HabboClubOffersMessageComposer = 3847;//1027;//480 // TODO

        public const int ClubWindowNewTest = 2416;
        //Mega Offer
        public const int TargetOfferMessageComposer = 2621;

        //Welcome Notif
        public const int WelcomeAlertComposer = 2954;
        public const int HabboClubCenterInfoMessageComposer = 3860;

        //Crafting 
        public const int CraftableProductsMessageComposer = 690; //correct
        public const int CraftingResultMessageComposer = 3378;//correct
        public const int CraftingFoundMessageComposer = 2146;//correct
        public const int CraftingRecipeMessageComposer = 3904;//correct


        public const int AvailableCraftingRecipeComposer = 1195;//TEST
        public const int CraftingCookConfirmationComposer = 1305;//

        //NUX
        public const int NuxInstructionComposer = 3445; //2954

        // Avatar
        public const int WardrobeMessageComposer = 1137;//2959

        // Catalog
        public const int CatalogIndexMessageComposer = 1222;//2140
        public const int CatalogItemDiscountMessageComposer = 2987;//796
        public const int PurchaseOKMessageComposer = 2513;//1450
        public const int CatalogOfferMessageComposer = 2072;//1757
        public const int CatalogPageMessageComposer = 3316;//3277
        public const int CatalogUpdatedMessageComposer = 2253;//1411
        public const int SellablePetBreedsMessageComposer = 2240;//2333
        public const int GroupFurniConfigMessageComposer = 2525;//3388
        public const int PresentDeliverErrorMessageComposer = 1429;//1971

        // Quests
        public const int QuestListMessageComposer = 1827;//3436
        public const int QuestCompletedMessageComposer = 1950;//3715
        public const int QuestAbortedMessageComposer = 963;//182
        public const int QuestStartedMessageComposer = 3891;//3281

        // Room Avatar
        public const int ActionMessageComposer = 3064;//3349
        public const int SleepMessageComposer = 2050;//2306
        public const int DanceMessageComposer = 1872;//130
        public const int CarryObjectMessageComposer = 1755;//2106
        public const int AvatarEffectMessageComposer = 362;//2062

        // Room Chat
        public const int ChatMessageComposer = 1659;//2785
        public const int ShoutMessageComposer = 2765;//2888
        public const int WhisperMessageComposer = 1899;//1400
        public const int FloodControlMessageComposer = 1889;//803
        public const int UserTypingMessageComposer = 3170;//1727

        // Room Engine
        public const int UsersMessageComposer = 1031;//3857
        public const int FurnitureAliasesMessageComposer = 29;//2159
        public const int ObjectAddMessageComposer = 824;//2076
        public const int ObjectsMessageComposer = 1147;//2783
        public const int ObjectUpdateMessageComposer = 2880;//1104
        public const int ObjectRemoveMessageComposer = 2993;//2362
        public const int SlideObjectBundleMessageComposer = 2561;//330
        public const int ItemsMessageComposer = 877;//580
        public const int ItemAddMessageComposer = 2251;//2236
        public const int ItemUpdateMessageComposer = 2582;//3408
        public const int ItemRemoveMessageComposer = 3762;//209

        // Room Session
        public const int RoomForwardMessageComposer = 1048;//3289
        public const int RoomReadyMessageComposer = 1098;//768
        public const int OpenConnectionMessageComposer = 3908;//3566
        public const int CloseConnectionMessageComposer = 2260;//726
        public const int FlatAccessibleMessageComposer = 2557;//735
        public const int CantConnectMessageComposer = 748;//200

        // Room Permissions
        public const int YouAreControllerMessageComposer = 231;//680
        public const int YouAreNotControllerMessageComposer = 2630;//1068
        public const int YouAreOwnerMessageComposer = 3976;//1932

        // Room Settings
        public const int RoomSettingsDataMessageComposer = 3133;//3361
        public const int RoomSettingsSavedMessageComposer = 1057;//3865
        public const int FlatControllerRemovedMessageComposer = 3470;//1501
        public const int FlatControllerAddedMessageComposer = 419;//3493
        public const int RoomRightsListMessageComposer = 1865;//225

        // Room Furniture
        public const int HideWiredConfigMessageComposer = 3620;//2430
        public const int WiredEffectConfigMessageComposer = 3535;//1428
        public const int WiredConditionConfigMessageComposer = 1234;//1775
        public const int WiredTriggerConfigMessageComposer = 3175;//21
        public const int MoodlightConfigMessageComposer = 2104;//1540
        public const int GroupFurniSettingsMessageComposer = 853;//3755
        public const int OpenGiftMessageComposer = 1090;//862

        // Navigator
        public const int UpdateFavouriteRoomMessageComposer = 1261;//3016
        public const int NavigatorLiftedRoomsMessageComposer = 3580;//1568
        public const int NavigatorPreferencesMessageComposer = 735;//3617
        public const int NavigatorFlatCatsMessageComposer = 2144;//1265
        public const int NavigatorMetaDataParserMessageComposer = 3830;//1071
        public const int NavigatorCollapsedCategoriesMessageComposer = 966;//232

        // Messenger
        public const int BuddyListMessageComposer = 758;//2900
        public const int BuddyRequestsMessageComposer = 1783;//177
        public const int NewBuddyRequestMessageComposer = 3779;//1525

        // Moderation
        public const int ModeratorInitMessageComposer = 3781;//2120
        public const int ModeratorUserRoomVisitsMessageComposer = 161;//1282
        public const int ModeratorRoomChatlogMessageComposer = 2564;//3561
        public const int ModeratorUserInfoMessageComposer = 3375;//3234
        public const int ModeratorSupportTicketResponseMessageComposer = 1212;//2651
        public const int ModeratorUserChatlogMessageComposer = 583;//2812
        public const int ModeratorRoomInfoMessageComposer = 467;//2318
        public const int ModeratorSupportTicketMessageComposer = 2027;//1258
        public const int ModeratorTicketChatlogMessageComposer = 935;//3637
        public const int CallForHelpPendingCallsMessageComposer = 1733;//2460
        public const int CfhTopicsInitMessageComposer = 1762;//1094

        // Inventory
        public const int CreditBalanceMessageComposer = 1662;//1958
        public const int BadgesMessageComposer = 2220;//2943
        public const int FurniListAddMessageComposer = 466;//2020
        public const int FurniListNotificationMessageComposer = 700;//439
        public const int FurniListRemoveMessageComposer = 2278;//3968
        public const int FurniListMessageComposer = 1307;//3640
        public const int FurniListUpdateMessageComposer = 1521;//1619
        public const int AvatarEffectsMessageComposer = 350;//1684
        public const int AvatarEffectActivatedMessageComposer = 2642;//545
        public const int AvatarEffectExpiredMessageComposer = 929;//2673
        public const int AvatarEffectAddedMessageComposer = 1137;//2959
        public const int TradingErrorMessageComposer = 962;//2484
        public const int TradingAcceptMessageComposer = 3467;//969
        public const int TradingStartMessageComposer = 372;//2527
        public const int TradingUpdateMessageComposer = 2364;//2088
        public const int TradingClosedMessageComposer = 2911;//1436
        public const int TradingCompleteMessageComposer = 2647;//2288
        public const int TradingConfirmedMessageComposer = 3467;//969
        public const int TradingFinishMessageComposer = 1363;//3443

        // Inventory Achievements
        public const int AchievementsMessageComposer = 1658;//1801
        public const int AchievementScoreMessageComposer = 3221;//1115
        public const int AchievementUnlockedMessageComposer = 811;//3385
        public const int AchievementProgressedMessageComposer = 2098;//2749

        // Notifications
        public const int ActivityPointsMessageComposer = 992;//3318
        public const int HabboActivityPointNotificationMessageComposer = 1546;//543

        // Users
        public const int ScrSendUserInfoMessageComposer = 1925;//826
        public const int IgnoredUsersMessageComposer = 2074;//2157

        // Groups
        public const int UnknownGroupMessageComposer = 1309;//1136
        public const int GroupMembershipRequestedMessageComposer = 1576;//2472
        public const int ManageGroupMessageComposer = 991;//230
        public const int HabboGroupBadgesMessageComposer = 84;//711
        public const int NewGroupInfoMessageComposer = 2197;//815
        public const int GroupInfoMessageComposer = 1530;//3712
        public const int GroupCreationWindowMessageComposer = 2815;//1062
        public const int SetGroupIdMessageComposer = 3437;//364
        public const int GroupMembersMessageComposer = 3602;//1401
        public const int UpdateFavouriteGroupMessageComposer = 3293;//2000
        public const int GroupMemberUpdatedMessageComposer = 2896;//3911
        public const int RefreshFavouriteGroupMessageComposer = 3611;//149

        // Group Forums
        public const int ForumsListDataMessageComposer = 2054;//1539
        public const int ForumDataMessageComposer = 1331;//91
        public const int ThreadCreatedMessageComposer = 306;//2675
        public const int ThreadDataMessageComposer = 3183;//2526
        public const int ThreadsListDataMessageComposer = 1501;//1056
        public const int ThreadUpdatedMessageComposer = 2265;//951
        public const int ThreadReplyMessageComposer = 2406;//1003

        // Sound
        public const int SoundSettingsMessageComposer = 903;//1949

        public const int QuestionParserMessageComposer = 2571;//1163
        public const int AvatarAspectUpdateMessageComposer = 125;//884;
        public const int HelperToolMessageComposer = 3757;//3610
        public const int RoomErrorNotifMessageComposer = 415;//2355
        public const int FollowFriendFailedMessageComposer = 1157;//3469

        public const int FindFriendsProcessResultMessageComposer = 1079;//2921
        public const int UserChangeMessageComposer = 50;//2248
        public const int FloorHeightMapMessageComposer = 2100;//1819
        public const int RoomInfoUpdatedMessageComposer = 3246;//3743
        public const int MessengerErrorMessageComposer = 3143;//880
        public const int MarketplaceCanMakeOfferResultMessageComposer = 1988;//2452
        public const int GameAccountStatusMessageComposer = 773;//3750
        public const int GuestRoomSearchResultMessageComposer = 762;//1634
        public const int NewUserExperienceGiftOfferMessageComposer = 1223;//2029
        public const int UpdateUsernameMessageComposer = 2266;//3461
        public const int VoucherRedeemOkMessageComposer = 2462;//2809
        public const int FigureSetIdsMessageComposer = 2837;//1811
        public const int StickyNoteMessageComposer = 104;//344
        public const int UserRemoveMessageComposer = 2756;//3839
        public const int GetGuestRoomResultMessageComposer = 836;//306
        public const int DoorbellMessageComposer = 698;//2068

        public const int GiftWrappingConfigurationMessageComposer = 3419;//766
        public const int GetRelationshipsMessageComposer = 246;//112
        public const int FriendNotificationMessageComposer = 2183;//3024
        public const int BadgeEditorPartsMessageComposer = 2910;//2839
        public const int TraxSongInfoMessageComposer = 182;//1159
        public const int PostUpdatedMessageComposer = 2479;//1180
        public const int UserUpdateMessageComposer = 2241;//3559
        public const int MutedMessageComposer = 1671;//2246
        public const int MarketplaceConfigurationMessageComposer = 478;//1817
        public const int CheckGnomeNameMessageComposer = 572;//3228
        public const int OpenBotActionMessageComposer = 2343;//464
        public const int FavouritesMessageComposer = 1422;//3267
        public const int TalentLevelUpMessageComposer = 2063;//3150

        public const int BCBorrowedItemsMessageComposer = 554;//1043
        public const int UserTagsMessageComposer = 2274;//940
        public const int CampaignMessageComposer = 1621;//2394
        public const int RoomEventMessageComposer = 2725;//1587
        public const int MarketplaceItemStatsMessageComposer = 1390;//480
        public const int HabboSearchResultMessageComposer = 3272;//2823
        public const int PetHorseFigureInformationMessageComposer = 1845;//2926
        public const int PetInventoryMessageComposer = 78;//1988
        public const int PongMessageComposer = 3101;//1240
        public const int RentableSpaceMessageComposer = 66;//2323
        public const int GetYouTubePlaylistMessageComposer = 3653;//1354
        public const int RespectNotificationMessageComposer = 1785;//1818
        public const int RecyclerRewardsMessageComposer = 2775;//1604
        public const int GetRoomBannedUsersMessageComposer = 3521;//1810
        public const int RoomRatingMessageComposer = 2019;//2454
        public const int PlayableGamesMessageComposer = 3525;//3076
        public const int TalentTrackLevelMessageComposer = 3655;//700
        public const int JoinQueueMessageComposer = 3674;//3139
        public const int MarketPlaceOwnOffersMessageComposer = 88;//1892
        public const int PetBreedingMessageComposer = 746;//528
        public const int SubmitBullyReportMessageComposer = 3743;//47
        public const int UserNameChangeMessageComposer = 1568;//574
        public const int LoveLockDialogueMessageComposer = 3884;//1157
        public const int SendBullyReportMessageComposer = 2488;//39
        public const int VoucherRedeemErrorMessageComposer = 2650;//2279
        public const int PurchaseErrorMessageComposer = 708;//1331
        public const int UnknownCalendarMessageComposer = 540;//128
        public const int FriendListUpdateMessageComposer = 1382;//1190

        public const int UserFlatCatsMessageComposer = 845;//3379
        public const int UpdateFreezeLivesMessageComposer = 581;//2998
        public const int UnbanUserFromRoomMessageComposer = 2945;//3710
        public const int PetTrainingPanelMessageComposer = 3044;//546
        public const int LoveLockDialogueCloseMessageComposer = 3484;//1767
        public const int BuildersClubMembershipMessageComposer = 1505;//820
        public const int FlatAccessDeniedMessageComposer = 3344;//797
        public const int LatencyResponseMessageComposer = 2485;//942
        public const int HabboUserBadgesMessageComposer = 1185;//3269
        public const int HeightMapMessageComposer = 3973;//1232

        public const int CanCreateRoomMessageComposer = 2221;//3568
        public const int InstantMessageErrorMessageComposer = 1070;//945
        public const int GnomeBoxMessageComposer = 3189;//1694
        public const int IgnoreStatusMessageComposer = 697;//2485
        public const int PetInformationMessageComposer = 1570;//3380
        public const int NavigatorSearchResultSetMessageComposer = 1036;//1089
        public const int ConcurrentUsersGoalProgressMessageComposer = 3097;//3782
        public const int VideoOffersRewardsMessageComposer = 3458;//1806
        public const int SanctionStatusMessageComposer = 1745;//3525
        public const int GetYouTubeVideoMessageComposer = 1955;//1022
        public const int CheckPetNameMessageComposer = 2599;//1760
        public const int RespectPetNotificationMessageComposer = 3577;//540
        public const int EnforceCategoryUpdateMessageComposer = 3519;//3714
        public const int CommunityGoalHallOfFameMessageComposer = 1359;//2629
        public const int FloorPlanFloorMapMessageComposer = 2151;//1855
        public const int SendGameInvitationMessageComposer = 1738;//2071
        public const int GiftWrappingErrorMessageComposer = 2041;//1385
        public const int PromoArticlesMessageComposer = 3845;//3015
        public const int Game1WeeklyLeaderboardMessageComposer = 371;//57
        public const int RentableSpacesErrorMessageComposer = 2919;//1255
        public const int AddExperiencePointsMessageComposer = 1139;//3791
        public const int OpenHelpToolMessageComposer = 1733;//2460
        public const int GetRoomFilterListMessageComposer = 3297;//1100
        public const int GameAchievementListMessageComposer = 1711;//2141
        public const int PromotableRoomsMessageComposer = 3698;//442
        public const int FloorPlanSendDoorMessageComposer = 1716;//1685
        public const int RoomEntryInfoMessageComposer = 2147;//3675
        public const int RoomNotificationMessageComposer = 2703;//3152
        public const int ClubGiftsMessageComposer = 3302;//2992
        public const int MOTDNotificationMessageComposer = 54;//1368
        public const int PopularRoomTagsResultMessageComposer = 2679;//1002
        public const int NewConsoleMessageMessageComposer = 3834;//984
        public const int RoomPropertyMessageComposer = 2558;//1897
        public const int MarketPlaceOffersMessageComposer = 886;//291
        public const int TalentTrackMessageComposer = 1512;//382
        public const int ProfileInformationMessageComposer = 3415;//3263
        public const int BadgeDefinitionsMessageComposer = 582;//1827
        public const int Game2WeeklyLeaderboardMessageComposer = 345;//275
        public const int NameChangeUpdateMessageComposer = 3319;//1226
        public const int RoomVisualizationSettingsMessageComposer = 3997;//3003
        public const int MarketplaceMakeOfferResultMessageComposer = 1390;//480
        public const int FlatCreatedMessageComposer = 912;//3001
        public const int BotInventoryMessageComposer = 1072;//3692
        public const int LoadGameMessageComposer = 3747;//652
        public const int UpdateMagicTileMessageComposer = 3857;//2811
        public const int CampaignCalendarDataMessageComposer = 906;//2276
        public const int MaintenanceStatusMessageComposer = 609;//3465
        public const int Game3WeeklyLeaderboardMessageComposer = 1330;//1326
        public const int GameListMessageComposer = 3824;//1220
        public const int RoomMuteSettingsMessageComposer = 2243;//1117
        public const int RoomInviteMessageComposer = 2983;//2138
        public const int LoveLockDialogueSetLockedMessageComposer = 3484;//1767
        public const int BroadcastMessageAlertMessageComposer = 777;//1751
        public const int MarketplaceCancelOfferResultMessageComposer = 88;//1892
        public const int NavigatorSettingsMessageComposer = 3503;//2477

        public const int MessengerInitMessageComposer = 3160;//1329

        //polls
        public const int PollAnswerComposer = 1723;

    }
}