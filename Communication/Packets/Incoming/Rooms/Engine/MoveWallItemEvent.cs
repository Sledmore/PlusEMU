using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class MoveWallItemEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session))
                return;

            int itemId = packet.PopInt();
            string wallPositionData = packet.PopString();

            Item item = room.GetRoomItemHandler().GetItem(itemId);

            if (item == null)
                return;

            try
            {
                string wallPos = room.GetRoomItemHandler().WallPositionCheck(":" + wallPositionData.Split(':')[1]);
                item.wallCoord = wallPos;
            }
            catch { return; }

            room.GetRoomItemHandler().UpdateItem(item);
            room.SendPacket(new ItemUpdateComposer(item, room.OwnerId));
        }
    }
}
