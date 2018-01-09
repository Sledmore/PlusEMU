using System;

using Plus.Utilities;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.Chat.Logs;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.Rooms.Chat.Styles;

namespace Plus.Communication.Packets.Incoming.Rooms.Chat
{
    public class ChatEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            string message = StringCharFilter.Escape(packet.PopString());
            if (message.Length > 100)
                message = message.Substring(0, 100);

            int colour = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(colour, out ChatStyle style) || style.RequiredRight.Length > 0 && !session.GetHabbo().GetPermissions().HasRight(style.RequiredRight))
                colour = 0;

            user.UnIdle();

            if (PlusEnvironment.GetUnixTimestamp() < session.GetHabbo().FloodTime && session.GetHabbo().FloodTime != 0)
                return;

            if (session.GetHabbo().TimeMuted > 0)
            {
                session.SendPacket(new MutedComposer(session.GetHabbo().TimeMuted));
                return;
            }

            if (!session.GetHabbo().GetPermissions().HasRight("room_ignore_mute") && room.CheckMute(session))
            {
                session.SendWhisper("Oops, you're currently muted.");
                return;
            }

            user.LastBubble = session.GetHabbo().CustomBubbleId == 0 ? colour : session.GetHabbo().CustomBubbleId;

            if (!session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                if (user.IncrementAndCheckFlood(out int muteTime))
                {
                    session.SendPacket(new FloodControlComposer(muteTime));
                    return;
                }
            }

            PlusEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new ChatlogEntry(session.GetHabbo().Id, room.Id, message, UnixTimestamp.GetNow(), session.GetHabbo(), room));

            if (message.StartsWith(":", StringComparison.CurrentCulture) && PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(session, message))
                return;

            if (PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckBannedWords(message))
            {
                session.GetHabbo().BannedPhraseCount++;
                if (session.GetHabbo().BannedPhraseCount >= Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("room.chat.filter.banned_phrases.chances")))
                {
                    PlusEnvironment.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.Username, session.GetHabbo().Username, "Spamming banned phrases (" + message + ")", PlusEnvironment.GetUnixTimestamp() + 78892200);
                    session.Disconnect();
                    return;
                }

                session.SendPacket(new ChatComposer(user.VirtualId, message, 0, colour));
                return;
            }

            if (!session.GetHabbo().GetPermissions().HasRight("word_filter_override"))
                message = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(message);


            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialChat);

            user.OnChat(user.LastBubble, message, false);
        }
    }
}