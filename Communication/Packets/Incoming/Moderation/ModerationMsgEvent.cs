using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerationMsgEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_alert"))
                return;

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();

            GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client == null)
                return;

            Client.SendNotification(Message);
        }
    }
}
