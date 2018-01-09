using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Users
{
    class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int userId = packet.PopInt();
            Habbo habbo = PlusEnvironment.GetHabboById(userId);
            if (habbo == null)
                return;

            session.SendPacket(new HabboUserBadgesComposer(habbo));
        }
    }
}