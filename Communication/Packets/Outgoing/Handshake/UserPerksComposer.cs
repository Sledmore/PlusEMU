using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class UserPerksComposer : ServerPacket
    {
        public UserPerksComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserPerksMessageComposer)
        {
            WriteInteger(15); // Count
            WriteString("USE_GUIDE_TOOL");
            WriteString("");
            WriteBoolean(false);
            WriteString("GIVE_GUIDE_TOURS");
            WriteString("requirement.unfulfilled.helper_le");
            WriteBoolean(false);
            WriteString("JUDGE_CHAT_REVIEWS");
            WriteString(""); // ??
            WriteBoolean(true);
            WriteString("VOTE_IN_COMPETITIONS");
            WriteString(""); // ??
            WriteBoolean(true);
            WriteString("CALL_ON_HELPERS");
            WriteString(""); // ??
            WriteBoolean(false);
            WriteString("CITIZEN");
            WriteString(""); // ??
            WriteBoolean(true);
            WriteString("TRADE");
            WriteString(""); // ??
            WriteBoolean(true);
            WriteString("HEIGHTMAP_EDITOR_BETA");
            WriteString(""); // ??
            WriteBoolean(false);
            WriteString("EXPERIMENTAL_CHAT_BETA");
            WriteString("requirement.unfulfilled.helper_level_2");
            WriteBoolean(true);
            WriteString("EXPERIMENTAL_TOOLBAR");
            WriteString(""); // ??
            WriteBoolean(true);
            WriteString("BUILDER_AT_WORK");
            WriteString(""); // ??
            WriteBoolean(true);
            WriteString("NAVIGATOR_PHASE_ONE_2014");
            WriteString(""); // ??
            WriteBoolean(false);
            WriteString("CAMERA");
            WriteString(""); // ??
            WriteBoolean(false);
            WriteString("NAVIGATOR_PHASE_TWO_2014");
            WriteString(""); // ??
            WriteBoolean(true);
            WriteString("MOUSE_ZOOM");
            WriteString(""); // ??
            WriteBoolean(true);
            WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
            WriteString(""); // ??
            WriteBoolean(false);
        }
    }
}