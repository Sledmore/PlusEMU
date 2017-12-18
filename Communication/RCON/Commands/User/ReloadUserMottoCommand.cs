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
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
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
                Room Room = client.GetHabbo().CurrentRoom;
                if (Room != null)
                {
                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(client.GetHabbo().Id);
                    if (User != null)
                    {
                        Room.SendPacket(new UserChangeComposer(User, false));
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
