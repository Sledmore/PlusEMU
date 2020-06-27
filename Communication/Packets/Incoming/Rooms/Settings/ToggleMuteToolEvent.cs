
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class ToggleMuteToolEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null || !room.CheckRights(session, true))
                return;

            room.RoomMuted = !room.RoomMuted;

            room.SendWhisper(room.RoomMuted ? "This room has been muted" : "This room has been unmuted");

            room.SendPacket(new RoomMuteSettingsComposer(room.RoomMuted));
        }
    }
}
