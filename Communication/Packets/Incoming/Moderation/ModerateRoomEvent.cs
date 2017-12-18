using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerateRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            Room Room = null;
            if(!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Packet.PopInt(), out Room))
                return;

            bool SetLock = Packet.PopInt() == 1;
            bool SetName = Packet.PopInt() == 1;
            bool KickAll = Packet.PopInt() == 1;

            if (SetName)
            {
                Room.RoomData.Name = "Inappropriate to Hotel Management";
                Room.RoomData.Description = "Inappropriate to Hotel Management";
            }

            if (SetLock)
                Room.RoomData.Access = RoomAccess.Doorbell;

            if (Room.Tags.Count > 0)
                Room.ClearTags();

            if (Room.RoomData.HasActivePromotion)
                Room.RoomData.EndPromotion();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (SetName && SetLock)
                    dbClient.RunQuery("UPDATE `rooms` SET `caption` = 'Inappropriate to Hotel Management', `description` = 'Inappropriate to Hotel Management', `tags` = '', `state` = '1' WHERE `id` = '" + Room.RoomId + "' LIMIT 1");
                else if (SetName && !SetLock)
                    dbClient.RunQuery("UPDATE `rooms` SET `caption` = 'Inappropriate to Hotel Management', `description` = 'Inappropriate to Hotel Management', `tags` = '' WHERE `id` = '" + Room.RoomId + "' LIMIT 1");
                else if (!SetName && SetLock)
                    dbClient.RunQuery("UPDATE `rooms` SET `state` = '1', `tags` = '' WHERE `id` = '" + Room.RoomId + "' LIMIT 1");
            }

            Room.SendPacket(new RoomSettingsSavedComposer(Room.RoomId));
            Room.SendPacket(new RoomInfoUpdatedComposer(Room.RoomId));

            if (KickAll)
            {
                foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (RoomUser == null || RoomUser.IsBot)
                        continue;

                    if (RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null)
                        continue;

                    if (RoomUser.GetClient().GetHabbo().Rank >= Session.GetHabbo().Rank || RoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                        continue;

                    Room.GetRoomUserManager().RemoveUserFromRoom(RoomUser.GetClient(), true, false);
                }
            }
        }
    }
}
