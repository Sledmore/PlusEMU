using System;
using System.Linq;
using System.Text;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.Communication.Packets.Incoming.Inventory.Purse
{
    class GetCreditsInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            Session.SendPacket(new ActivityPointsComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Diamonds, Session.GetHabbo().GOTWPoints));
        }
    }
}
