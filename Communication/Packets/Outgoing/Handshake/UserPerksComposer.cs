using System;
using System.Collections.Generic;

using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class UserPerksComposer : ServerPacket
    {
        public UserPerksComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserPerksMessageComposer)
        {
            base.WriteInteger(15); // Count
            base.WriteString("USE_GUIDE_TOOL");
            base.WriteString("");
            base.WriteBoolean(false);
            base.WriteString("GIVE_GUIDE_TOURS");
            base.WriteString("requirement.unfulfilled.helper_le");
            base.WriteBoolean(false);
            base.WriteString("JUDGE_CHAT_REVIEWS");
            base.WriteString(""); // ??
            base.WriteBoolean(true);
            base.WriteString("VOTE_IN_COMPETITIONS");
            base.WriteString(""); // ??
            base.WriteBoolean(true);
            base.WriteString("CALL_ON_HELPERS");
            base.WriteString(""); // ??
            base.WriteBoolean(false);
            base.WriteString("CITIZEN");
            base.WriteString(""); // ??
            base.WriteBoolean(true);
            base.WriteString("TRADE");
            base.WriteString(""); // ??
            base.WriteBoolean(true);
            base.WriteString("HEIGHTMAP_EDITOR_BETA");
            base.WriteString(""); // ??
            base.WriteBoolean(false);
            base.WriteString("EXPERIMENTAL_CHAT_BETA");
            base.WriteString("requirement.unfulfilled.helper_level_2");
            base.WriteBoolean(true);
            base.WriteString("EXPERIMENTAL_TOOLBAR");
            base.WriteString(""); // ??
            base.WriteBoolean(true);
            base.WriteString("BUILDER_AT_WORK");
            base.WriteString(""); // ??
            base.WriteBoolean(true);
            base.WriteString("NAVIGATOR_PHASE_ONE_2014");
            base.WriteString(""); // ??
            base.WriteBoolean(false);
            base.WriteString("CAMERA");
            base.WriteString(""); // ??
            base.WriteBoolean(false);
            base.WriteString("NAVIGATOR_PHASE_TWO_2014");
            base.WriteString(""); // ??
            base.WriteBoolean(true);
            base.WriteString("MOUSE_ZOOM");
            base.WriteString(""); // ??
            base.WriteBoolean(true);
            base.WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
            base.WriteString(""); // ??
            base.WriteBoolean(false);
        }
    }
}