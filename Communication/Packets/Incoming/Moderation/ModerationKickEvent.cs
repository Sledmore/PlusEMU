using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerationKickEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_kick"))
                return;

            int userId = packet.PopInt();
            packet.PopString(); //message

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.GetHabbo() == null || client.GetHabbo().CurrentRoomId < 1 || client.GetHabbo().Id == session.GetHabbo().Id)
                return;

            if (client.GetHabbo().Rank >= session.GetHabbo().Rank)
            {
                session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("moderation.kick.disallowed"));
                return;
            }

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            room.GetRoomUserManager().RemoveUserFromRoom(client, true);
        }
    }
}
