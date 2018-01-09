using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class DiceOffEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            Item item = room.GetRoomItemHandler().GetItem(packet.PopInt());
            if (item == null)
                return;

            bool hasRights = room.CheckRights(session);

            item.Interactor.OnTrigger(session, item, -1, hasRights);
        }
    }
}
