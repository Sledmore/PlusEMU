using System.Linq;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.Communication.Packets.Incoming.Users
{
    class ChangeNameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            Room Room = session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Username);
            if (User == null)
                return;

            string NewName = packet.PopString();
            string OldName = session.GetHabbo().Username;

            if (NewName == OldName)
            {
                session.GetHabbo().ChangeName(OldName);
                session.SendPacket(new UpdateUsernameComposer(NewName));
                return;
            }

            if (!CanChangeName(session.GetHabbo()))
            {
                session.SendNotification("Oops, it appears you currently cannot change your username!");
                return;
            }

            bool InUse = false;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `users` WHERE `username` = @name LIMIT 1");
                dbClient.AddParameter("name", NewName);
                InUse = dbClient.GetInteger() == 1;
            }

            char[] Letters = NewName.ToLower().ToCharArray();
            string AllowedCharacters = "abcdefghijklmnopqrstuvwxyz.,_-;:?!1234567890";

            foreach (char Chr in Letters)
            {
                if (!AllowedCharacters.Contains(Chr))
                {
                    return;
                }
            }

            if (!session.GetHabbo().GetPermissions().HasRight("mod_tool") && NewName.ToLower().Contains("mod") || NewName.ToLower().Contains("adm") || NewName.ToLower().Contains("admin")
                || NewName.ToLower().Contains("m0d") || NewName.ToLower().Contains("mob") || NewName.ToLower().Contains("m0b"))
                return;
            else if (!NewName.ToLower().Contains("mod") && (session.GetHabbo().Rank == 2 || session.GetHabbo().Rank == 3))
                return;
            else if (NewName.Length > 15)
                return;
            else if (NewName.Length < 3)
                return;
            else if (InUse)
                return;
            else
            {
                if (!PlusEnvironment.GetGame().GetClientManager().UpdateClientUsername(session, OldName, NewName))
                {
                    session.SendNotification("Oops! An issue occoured whilst updating your username.");
                    return;
                }

                session.GetHabbo().ChangingName = false;

                Room.GetRoomUserManager().RemoveUserFromRoom(session, true, false);

                session.GetHabbo().ChangeName(NewName);
                session.GetHabbo().GetMessenger().OnStatusChanged(true);

                session.SendPacket(new UpdateUsernameComposer(NewName));
                Room.SendPacket(new UserNameChangeComposer(Room.Id, User.VirtualId, NewName));

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `logs_client_namechange` (`user_id`,`new_name`,`old_name`,`timestamp`) VALUES ('" + session.GetHabbo().Id + "', @name, '" + OldName + "', '" + PlusEnvironment.GetUnixTimestamp() + "')");
                    dbClient.AddParameter("name", NewName);
                    dbClient.RunQuery();
                }


                foreach (Room room in PlusEnvironment.GetGame().GetRoomManager().GetRooms().ToList())
                {
                    if (room == null || room.OwnerId != session.GetHabbo().Id || room.OwnerName == NewName)
                        continue;

                    room.OwnerName = NewName;
                    room.SendPacket(new RoomInfoUpdatedComposer(room.Id));
                }

                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_Name", 1);

               session.SendPacket(new RoomForwardComposer(Room.Id));
            }
        }

        private static bool CanChangeName(Habbo Habbo)
        {

            if (Habbo.Rank == 1 && Habbo.VIPRank == 0 && Habbo.LastNameChange == 0)
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 1 && (Habbo.LastNameChange == 0 || (PlusEnvironment.GetUnixTimestamp() + 604800) > Habbo.LastNameChange))
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 2 && (Habbo.LastNameChange == 0 || (PlusEnvironment.GetUnixTimestamp() + 86400) > Habbo.LastNameChange))
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 3)
                return true;
            else if (Habbo.GetPermissions().HasRight("mod_tool"))
                return true;

            return false;
        }
    }
}