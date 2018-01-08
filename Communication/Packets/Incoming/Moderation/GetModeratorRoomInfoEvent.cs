using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int roomId = Packet.PopInt();

            RoomData data = null;
            if (!RoomFactory.TryGetData(roomId, out data))
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out Room Room))
                return;

            Session.SendPacket(new ModeratorRoomInfoComposer(data, (Room.GetRoomUserManager().GetRoomUserByHabbo(data.OwnerName) != null)));
        }
    }
}
