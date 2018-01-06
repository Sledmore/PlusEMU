using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.Communication.Rcon.Commands.User
{
    class ReloadUserMottoCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload the users motto from the database."; }
        }

        public string Parameters
        {
           get { return "%userId%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            if (!int.TryParse(parameters[0], out int userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `motto` FROM `users` WHERE `id` = @userID LIMIT 1");
                dbClient.AddParameter("userID", userId);
                client.GetHabbo().Motto = dbClient.GetString();
            }

            // If we're in a room, we cannot really send the packets, so flag this as completed successfully, since we already updated it.
            if (!client.GetHabbo().InRoom)
            {
                return true;
            }
            else
            {
                //We are in a room, let's try to run the packets.
                Room room = client.GetHabbo().CurrentRoom;
                if (room != null)
                {
                    RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(client.GetHabbo().Id);
                    if (user != null)
                    {
                        room.SendPacket(new UserChangeComposer(user, false));
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
