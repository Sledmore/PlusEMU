using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            int roomId = packet.PopInt();

            Room room = null;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryLoadRoom(roomId, out room))
                return;

            if (!room.CheckRights(session, true))
                return;

            session.SendPacket(new RoomSettingsDataComposer(room));
        }
    }
}
