using System;
using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Utilities;
using Plus.HabboHotel.Rooms.Chat.Styles;
using Plus.HabboHotel.Rooms.Chat.Logs;

namespace Plus.Communication.Packets.Incoming.Rooms.Chat
{
    public class WhisperEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            if (!session.GetHabbo().GetPermissions().HasRight("mod_tool") && room.CheckMute(session))
            {
                session.SendWhisper("Oops, you're currently muted.");
                return;
            }

            if (PlusEnvironment.GetUnixTimestamp() < session.GetHabbo().FloodTime && session.GetHabbo().FloodTime != 0)
                return;

            string Params = packet.PopString();
            string toUser = Params.Split(' ')[0];
            string message = Params.Substring(toUser.Length + 1);
            int colour = packet.PopInt();

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            RoomUser user2 = room.GetRoomUserManager().GetRoomUserByHabbo(toUser);
            if (user2 == null)
                return;

            if (session.GetHabbo().TimeMuted > 0)
            {
                session.SendPacket(new MutedComposer(session.GetHabbo().TimeMuted));
                return;
            }

            if (!session.GetHabbo().GetPermissions().HasRight("word_filter_override"))
                message = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(message);

            if (!PlusEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(colour, out ChatStyle style) || style.RequiredRight.Length > 0 && !session.GetHabbo().GetPermissions().HasRight(style.RequiredRight))
                colour = 0;

            user.LastBubble = session.GetHabbo().CustomBubbleId == 0 ? colour : session.GetHabbo().CustomBubbleId;

            if (!session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                if (user.IncrementAndCheckFlood(out int muteTime))
                {
                    session.SendPacket(new FloodControlComposer(muteTime));
                    return;
                }
            }

            if (!user2.GetClient().GetHabbo().ReceiveWhispers && !session.GetHabbo().GetPermissions().HasRight("room_whisper_override"))
            {
                session.SendWhisper("Oops, this user has their whispers disabled!");
                return;
            }
            
            PlusEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new ChatlogEntry(session.GetHabbo().Id, room.Id, "<Whisper to " + toUser + ">: " + message, UnixTimestamp.GetNow(), session.GetHabbo(), room));

            if (PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckBannedWords(message))
            {
                session.GetHabbo().BannedPhraseCount++;
                if (session.GetHabbo().BannedPhraseCount >= Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("room.chat.filter.banned_phrases.chances")))
                {
                    PlusEnvironment.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.Username, session.GetHabbo().Username, "Spamming banned phrases (" + message + ")", PlusEnvironment.GetUnixTimestamp() + 78892200);
                    session.Disconnect();
                    return;
                }
                session.SendPacket(new WhisperComposer(user.VirtualId, message, 0, user.LastBubble));
                return;
            }


            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialChat);

            user.UnIdle();
            user.GetClient().SendPacket(new WhisperComposer(user.VirtualId, message, 0, user.LastBubble));

            if (!user2.IsBot && user2.UserId != user.UserId)
            {
                if (!user2.GetClient().GetHabbo().GetIgnores().IgnoredUserIds().Contains(session.GetHabbo().Id))
                {
                    user2.GetClient().SendPacket(new WhisperComposer(user.VirtualId, message, 0, user.LastBubble));
                }
            }
 
            List<RoomUser> toNotify = room.GetRoomUserManager().GetRoomUserByRank(2);
            if (toNotify.Count > 0)
            {
                foreach (RoomUser notifiable in toNotify)
                {
                    if (notifiable != null && notifiable.HabboId != user2.HabboId && notifiable.HabboId != user.HabboId)
                    {
                        if (notifiable.GetClient() != null && notifiable.GetClient().GetHabbo() != null && !notifiable.GetClient().GetHabbo().IgnorePublicWhispers)
                        {
                            notifiable.GetClient().SendPacket(new WhisperComposer(user.VirtualId, "[Whisper to " + toUser + "] " + message, 0, user.LastBubble));
                        }
                    }
                }
            }
        }
    }
}