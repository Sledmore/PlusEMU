using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.AvatarEffects
{
    class AvatarEffectSelectedEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int effectId = packet.PopInt();
            if (effectId < 0)
                effectId = 0;

            if (!session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            if (effectId != 0 && session.GetHabbo().Effects().HasEffect(effectId, true))
                user.ApplyEffect(effectId);
        }
    }
}
