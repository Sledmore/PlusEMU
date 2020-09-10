using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class UserPerksComposer : MessageComposer
    {
        public Habbo Habbo { get; }

        public UserPerksComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserPerksMessageComposer)
        {
            this.Habbo = Habbo;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(15); // Count
            packet.WriteString("USE_GUIDE_TOOL");
            packet.WriteString("");
            packet.WriteBoolean(false);
            packet.WriteString("GIVE_GUIDE_TOURS");
            packet.WriteString("requirement.unfulfilled.helper_le");
            packet.WriteBoolean(false);
            packet.WriteString("JUDGE_CHAT_REVIEWS");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("VOTE_IN_COMPETITIONS");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("CALL_ON_HELPERS");
            packet.WriteString(""); // ??
            packet.WriteBoolean(false);
            packet.WriteString("CITIZEN");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("TRADE");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("HEIGHTMAP_EDITOR_BETA");
            packet.WriteString(""); // ??
            packet.WriteBoolean(false);
            packet.WriteString("EXPERIMENTAL_CHAT_BETA");
            packet.WriteString("requirement.unfulfilled.helper_level_2");
            packet.WriteBoolean(true);
            packet.WriteString("EXPERIMENTAL_TOOLBAR");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("BUILDER_AT_WORK");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("NAVIGATOR_PHASE_ONE_2014");
            packet.WriteString(""); // ??
            packet.WriteBoolean(false);
            packet.WriteString("CAMERA");
            packet.WriteString(""); // ??
            packet.WriteBoolean(false);
            packet.WriteString("NAVIGATOR_PHASE_TWO_2014");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("MOUSE_ZOOM");
            packet.WriteString(""); // ??
            packet.WriteBoolean(true);
            packet.WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
            packet.WriteString(""); // ??
            packet.WriteBoolean(false);
        }
    }
}