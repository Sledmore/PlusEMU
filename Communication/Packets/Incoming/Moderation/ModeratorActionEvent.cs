using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModeratorActionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().GetPermissions().HasRight("mod_caution"))
                return;

            if (!session.GetHabbo().InRoom)
                return;

            Room currentRoom = session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;

            int alertMode = packet.PopInt(); 
            string alertMessage = packet.PopString();
            bool isCaution = alertMode != 3;

            alertMessage = isCaution ? "Caution from Moderator:\n\n" + alertMessage : "Message from Moderator:\n\n" + alertMessage;
            session.GetHabbo().CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(alertMessage));
        }
    }
}
