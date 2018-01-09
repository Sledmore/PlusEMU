using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Purse
{
    class GetHabboClubWindowEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
           // Session.SendNotification("Habbo Club is free for all members, enjoy!");
        }
    }
}
