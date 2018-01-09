using System.Text;


using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class RoomCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_room"; }
        }

        public string Parameters
        {
            get { return "push/pull/enables/respect"; }
        }

        public string Description
        {
            get { return "Gives you the ability to enable or disable basic room commands."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, you must choose a room option to disable.");
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, only the room owner or staff can use this command.");
                return;
            }

            string Option = Params[1];
            switch (Option)
            {
                case "list":
                {
                    StringBuilder List = new StringBuilder("");
                    List.AppendLine("Room Command List");
                    List.AppendLine("-------------------------");
                    List.AppendLine("Pet Morphs: " + (Room.PetMorphsAllowed == true ? "enabled" : "disabled"));
                    List.AppendLine("Pull: " + (Room.PullEnabled == true ? "enabled" : "disabled"));
                    List.AppendLine("Push: " + (Room.PushEnabled == true ? "enabled" : "disabled"));
                    List.AppendLine("Super Pull: " + (Room.SuperPullEnabled == true ? "enabled" : "disabled"));
                    List.AppendLine("Super Push: " + (Room.SuperPushEnabled == true ? "enabled" : "disabled"));
                    List.AppendLine("Respect: " + (Room.RespectNotificationsEnabled == true ? "enabled" : "disabled"));
                    List.AppendLine("Enables: " + (Room.EnablesEnabled == true ? "enabled" : "disabled"));
                    Session.SendNotification(List.ToString());
                    break;
                }

                case "push":
                    {
                        Room.PushEnabled = !Room.PushEnabled;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `push_enabled` = @PushEnabled WHERE `id` = '" + Room.Id +"' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", PlusEnvironment.BoolToEnum(Room.PushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Push mode is now " + (Room.PushEnabled == true ? "enabled!" : "disabled!"));
                        break;
                    }

                case "spush":
                    {
                        Room.SuperPushEnabled = !Room.SuperPushEnabled;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spush_enabled` = @PushEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", PlusEnvironment.BoolToEnum(Room.SuperPushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Super Push mode is now " + (Room.SuperPushEnabled == true ? "enabled!" : "disabled!"));
                        break;
                    }

                case "spull":
                    {
                        Room.SuperPullEnabled = !Room.SuperPullEnabled;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", PlusEnvironment.BoolToEnum(Room.SuperPullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Super Pull mode is now " + (Room.SuperPullEnabled == true ? "enabled!" : "disabled!"));
                        break;
                    }

                case "pull":
                    {
                        Room.PullEnabled = !Room.PullEnabled;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", PlusEnvironment.BoolToEnum(Room.PullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Pull mode is now " + (Room.PullEnabled == true ? "enabled!" : "disabled!"));
                        break;
                    }

                case "enable":
                case "enables":
                    {
                        Room.EnablesEnabled = !Room.EnablesEnabled;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `enables_enabled` = @EnablesEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("EnablesEnabled", PlusEnvironment.BoolToEnum(Room.EnablesEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Enables mode set to " + (Room.EnablesEnabled == true ? "enabled!" : "disabled!"));
                        break;
                    }

                case "respect":
                    {
                        Room.RespectNotificationsEnabled = !Room.RespectNotificationsEnabled;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `respect_notifications_enabled` = @RespectNotificationsEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("RespectNotificationsEnabled", PlusEnvironment.BoolToEnum(Room.RespectNotificationsEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Respect notifications mode set to " + (Room.RespectNotificationsEnabled == true ? "enabled!" : "disabled!"));
                        break;
                    }

                case "pets":
                case "morphs":
                    {
                        Room.PetMorphsAllowed = !Room.PetMorphsAllowed;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pet_morphs_allowed` = @PetMorphsAllowed WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PetMorphsAllowed", PlusEnvironment.BoolToEnum(Room.PetMorphsAllowed));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Human pet morphs notifications mode set to " + (Room.PetMorphsAllowed == true ? "enabled!" : "disabled!"));
                        
                        if (!Room.PetMorphsAllowed)
                        {
                            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                            {
                                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                    continue;

                                User.GetClient().SendWhisper("The room owner has disabled the ability to use a pet morph in this room.");
                                if (User.GetClient().GetHabbo().PetId > 0)
                                {
                                    //Tell the user what is going on.
                                    User.GetClient().SendWhisper("Oops, the room owner has just disabled pet-morphs, un-morphing you.");
                                    
                                    //Change the users Pet Id.
                                    User.GetClient().GetHabbo().PetId = 0;

                                    //Quickly remove the old user instance.
                                    Room.SendPacket(new UserRemoveComposer(User.VirtualId));

                                    //Add the new one, they won't even notice a thing!!11 8-)
                                    Room.SendPacket(new UsersComposer(User));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
