using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Users;

namespace Plus.Communication.Packets.Incoming.Users
{
    class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();
            Habbo Habbo = PlusEnvironment.GetHabboById(UserId);
            if (Habbo == null)
                return;

            Session.SendPacket(new HabboUserBadgesComposer(Habbo));
        }
    }
}