using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ReleaseTicketEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int amount = packet.PopInt();

            for (int i = 0; i < amount; i++)
            {
                if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(packet.PopInt(), out ModerationTicket ticket))
                    continue;

                ticket.Moderator = null;
                PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.GetHabbo().Id, ticket), "mod_tool");
            }
        }
    }
}