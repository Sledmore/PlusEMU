using System;
using DotNetty.Transport.Channels;
using Plus.Core;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Interfaces;
using Plus.Communication.Packets.Outgoing.Sound;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;

using Plus.Communication.Encryption.Crypto.Prng;
using Plus.Communication.Packets.Outgoing;
using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.Communication.Packets.Outgoing.BuildersClub;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Subscriptions;
using Plus.HabboHotel.Permissions;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.Users.UserData;
using Plus.Network.Codec;
using System.Collections.Generic;

namespace Plus.HabboHotel.GameClients
{
    public class GameClient
    {
        private Habbo _habbo;
        public string MachineId;
        private bool _disconnected;
        private IChannelHandlerContext channel;
        public int PingCount { get; set; }

        public GameClient(IChannelHandlerContext context)
        {
            channel = context;
        }

        public bool TryAuthenticate(string authTicket)
        {
            try
            {
                UserData userData = UserDataFactory.GetUserData(authTicket, out byte errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking
                //Let's have a quick search for a ban before we successfully authenticate..
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(MachineId, out _))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.user != null)
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(userData.user.Username, out _))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().UsernameBanCheck(userData.user.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }
                #endregion

                if (userData.user == null) //Possible NPE
                {
                    return false;
                }

                PlusEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.UserId, userData.user.Username);
                _habbo = userData.user;
                if (_habbo != null)
                {
                    userData.user.Init(this, userData);

                    SendPacket(new AuthenticationOKComposer());
                    SendPacket(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    SendPacket(new NavigatorSettingsComposer(_habbo.HomeRoom));
                    SendPacket(new FavouritesComposer(userData.user.FavoriteRooms));
                    SendPacket(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingParts));
                    SendPacket(new UserRightsComposer(_habbo.Rank));
                    SendPacket(new AvailabilityStatusComposer());
                    SendPacket(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));
                    SendPacket(new BuildersClubMembershipComposer());
                    SendPacket(new CfhTopicsInitComposer(PlusEnvironment.GetGame().GetModerationManager().UserActionPresets));

                    SendPacket(new BadgeDefinitionsComposer(PlusEnvironment.GetGame().GetAchievementManager().Achievements));
                    SendPacket(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendbarState)));
                    //SendMessage(new TalentTrackLevelComposer());

                    if (GetHabbo().GetMessenger() != null)
                        GetHabbo().GetMessenger().OnStatusChanged(true);

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (_habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        _habbo.MachineId = MachineId;
                    }

                    if (PlusEnvironment.GetGame().GetPermissionManager().TryGetGroup(_habbo.Rank, out PermissionGroup group))
                    {
                        if (!String.IsNullOrEmpty(group.Badge))
                            if (!_habbo.GetBadgeComponent().HasBadge(group.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(group.Badge, true, this);
                    }

                    if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(_habbo.VIPRank, out SubscriptionData subData))
                    {
                        if (!String.IsNullOrEmpty(subData.Badge))
                        {
                            if (!_habbo.GetBadgeComponent().HasBadge(subData.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(subData.Badge, true, this);
                        }
                    }

                    if (!PlusEnvironment.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                        PlusEnvironment.GetGame().GetCacheManager().GenerateUser(_habbo.Id);

                    _habbo.Look = PlusEnvironment.GetFigureManager().ProcessFigure(_habbo.Look, _habbo.Gender, _habbo.GetClothing().GetClothingParts, true);
                    _habbo.InitProcess();
          
                    if (userData.user.GetPermissions().HasRight("mod_tickets"))
                    {
                        SendPacket(new ModeratorInitComposer(
                          PlusEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                          PlusEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                          PlusEnvironment.GetGame().GetModerationManager().GetTickets));
                    }

                    if (PlusEnvironment.GetSettingsManager().TryGetValue("user.login.message.enabled") == "1")
                        SendPacket(new MotdNotificationComposer(PlusEnvironment.GetLanguageManager().TryGetValue("user.login.message")));

                    PlusEnvironment.GetGame().GetRewardManager().CheckRewards(this);
                    return true;
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
            return false;
        }

        public void SendWhisper(string message, int colour = 0)
        {
            if (GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser user = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (user == null)
                return;

            SendPacket(new WhisperComposer(user.VirtualId, message, 0, (colour == 0 ? user.LastBubble : colour)));
        }

        public void SendNotification(string message)
        {
            SendPacket(new BroadcastMessageAlertComposer(message));
        }

        public void SendPacket(MessageComposer message)
        {
            channel.WriteAndFlushAsync(message);
        }

        public async void SendPacketsAsync(List<MessageComposer> messages)
        {
            foreach(MessageComposer message in messages)
            {
                await channel.WriteAsync(message);
            }
            channel.Flush();
        }

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            if (!_disconnected)
            {
                if (channel != null)
                    channel.CloseAsync();
                _disconnected = true;
            }
        }

        public void Dispose()
        {
            if (GetHabbo() != null)
                GetHabbo().OnDisconnect();

            MachineId = string.Empty;
            _disconnected = true;
            _habbo = null;
            channel.DisconnectAsync();
        }

        public void EnableEncryption(byte[] sharedKey)
        {
            channel.Channel.Pipeline.AddFirst("gameCrypto", new EncryptionDecoder(sharedKey));
        }
    }
}