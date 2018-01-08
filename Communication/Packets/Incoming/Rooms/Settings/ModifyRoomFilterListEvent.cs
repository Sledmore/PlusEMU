using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class ModifyRoomFilterListEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room instance = session.GetHabbo().CurrentRoom;
            if (instance == null)
                return;

            if (!instance.CheckRights(session))
                return;

            packet.PopInt(); //roomId
            bool added = packet.PopBoolean();
            string word = packet.PopString();

            if (added)
                instance.GetFilter().AddFilter(word);
            else
                instance.GetFilter().RemoveFilter(word);
        }
    }
}
