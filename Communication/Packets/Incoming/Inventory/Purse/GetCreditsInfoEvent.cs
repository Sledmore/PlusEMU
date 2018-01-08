using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.Communication.Packets.Incoming.Inventory.Purse
{
    class GetCreditsInfoEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));
            session.SendPacket(new ActivityPointsComposer(session.GetHabbo().Duckets, session.GetHabbo().Diamonds, session.GetHabbo().GOTWPoints));
        }
    }
}
