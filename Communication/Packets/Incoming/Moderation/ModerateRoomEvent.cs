using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerateRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(packet.PopInt(), out Room room))
                return;

            bool setLock = packet.PopInt() == 1;
            bool setName = packet.PopInt() == 1;
            bool kickAll = packet.PopInt() == 1;

            if (setName)
            {
                room.Name = "Inappropriate to Hotel Management";
                room.Description = "Inappropriate to Hotel Management";
            }

            if (setLock)
                room.Access = RoomAccess.Doorbell;

            if (room.Tags.Count > 0)
                room.ClearTags();

            if (room.HasActivePromotion)
                room.EndPromotion();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (setName && setLock)
                    dbClient.RunQuery("UPDATE `rooms` SET `caption` = 'Inappropriate to Hotel Management', `description` = 'Inappropriate to Hotel Management', `tags` = '', `state` = '1' WHERE `id` = '" + room.RoomId + "' LIMIT 1");
                else if (setName)
                    dbClient.RunQuery("UPDATE `rooms` SET `caption` = 'Inappropriate to Hotel Management', `description` = 'Inappropriate to Hotel Management', `tags` = '' WHERE `id` = '" + room.RoomId + "' LIMIT 1");
                else if (setLock)
                    dbClient.RunQuery("UPDATE `rooms` SET `state` = '1', `tags` = '' WHERE `id` = '" + room.RoomId + "' LIMIT 1");
            }

            room.SendPacket(new RoomSettingsSavedComposer(room.RoomId));
            room.SendPacket(new RoomInfoUpdatedComposer(room.RoomId));

            if (kickAll)
            {
                foreach (RoomUser roomUser in room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (roomUser == null || roomUser.IsBot)
                        continue;

                    if (roomUser.GetClient() == null || roomUser.GetClient().GetHabbo() == null)
                        continue;

                    if (roomUser.GetClient().GetHabbo().Rank >= session.GetHabbo().Rank || roomUser.GetClient().GetHabbo().Id == session.GetHabbo().Id)
                        continue;

                    room.GetRoomUserManager().RemoveUserFromRoom(roomUser.GetClient(), true);
                }
            }
        }
    }
}
