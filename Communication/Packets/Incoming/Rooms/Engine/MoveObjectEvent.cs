using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Quests;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class MoveObjectEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            int itemId = packet.PopInt();
            if (itemId == 0)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            Item item;
            if (room.Group != null)
            {
                if (!room.CheckRights(session, false, true))
                {
                    item = room.GetRoomItemHandler().GetItem(itemId);
                    if (item == null)
                        return;

                    session.SendPacket(new ObjectUpdateComposer(item, room.OwnerId));
                    return;
                }
            }
            else
            {
                if (!room.CheckRights(session))
                {
                    return;
                }
            }

            item = room.GetRoomItemHandler().GetItem(itemId);

            if (item == null)
                return;

            int x = packet.PopInt();
            int y = packet.PopInt();
            int rotation = packet.PopInt();

            if (x != item.GetX || y != item.GetY)
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniMove);

            if (rotation != item.Rotation)
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniRotate);

            if (!room.GetRoomItemHandler().SetFloorItem(session, item, x, y, rotation, false, false, true))
            {
                room.SendPacket(new ObjectUpdateComposer(item, room.OwnerId));
                return;
            }

            if (item.GetZ >= 0.1)
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniStack);
        }
    }
}