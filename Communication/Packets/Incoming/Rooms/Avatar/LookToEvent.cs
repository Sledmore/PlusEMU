using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.PathFinding;

namespace Plus.Communication.Packets.Incoming.Rooms.Avatar
{
    class LookToEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            if (user.IsAsleep)
                return;
            
            user.UnIdle();

            int x = packet.PopInt();
            int y = packet.PopInt();

            if ((x == user.X && y == user.Y) || user.IsWalking || user.RidingHorse)
                return;

            int rot = Rotation.Calculate(user.X, user.Y, x, y);

            user.SetRot(rot, false);
            user.UpdateNeeded = true;

            if (user.RidingHorse)
            {
                RoomUser horse = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByVirtualId(user.HorseID);
                if (horse != null)
                {
                    horse.SetRot(rot, false);
                    horse.UpdateNeeded = true;
                }
            }
        }
    }
}
