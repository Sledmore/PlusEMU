using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using System;
namespace Plus.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class EventAlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_event_alert";
            }
        }
        public string Parameters
        {
            get
            {
                return "";
            }
        }
        public string Description
        {
            get
            {
                return "Send a hotel alert for your event!";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session != null)
            {
                if (Room != null)
                {
                    if (Params.Length != 1)
                    {
                        Session.SendWhisper("Invalid command! :eventalert", 0);
                    }
                    else if (!PlusEnvironment.Event)
                    {
                        PlusEnvironment.GetGame().GetClientManager().SendPacket(new BroadcastMessageAlertComposer(":follow " + Session.GetHabbo().Username + " for events! win prizes!\r\n- " + Session.GetHabbo().Username, ""), "");
                        PlusEnvironment.lastEvent = DateTime.Now;
                        PlusEnvironment.Event = true;
                    }
                    else
                    {
                        TimeSpan timeSpan = DateTime.Now - PlusEnvironment.lastEvent;
                        if (timeSpan.Hours >= 1)
                        {
                            PlusEnvironment.GetGame().GetClientManager().SendPacket(new BroadcastMessageAlertComposer(":follow " + Session.GetHabbo().Username + " for events! win prizes!\r\n- " + Session.GetHabbo().Username, ""), "");
                            PlusEnvironment.lastEvent = DateTime.Now;
                        }
                        else
                        {
                            int num = checked(60 - timeSpan.Minutes);
                            Session.SendWhisper("Event Cooldown! " + num + " minutes left until another event can be hosted.", 0);
                        }
                    }
                }
            }
        }
    }
}
