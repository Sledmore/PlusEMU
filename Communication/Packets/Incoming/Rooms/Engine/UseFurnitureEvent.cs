using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;
using Plus.Communication.Packets.Outgoing.Rooms.Furni;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class UseFurnitureEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            int itemId = packet.PopInt();
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            bool hasRights = room.CheckRights(session, false, true);

            if (item.GetBaseItem().InteractionType == InteractionType.banzaitele)
                return;

            if (item.GetBaseItem().InteractionType == InteractionType.TONER)
            {
                if (!room.CheckRights(session, true))
                    return;
                
                room.TonerData.Enabled = room.TonerData.Enabled == 0 ? 1 : 0;

                room.SendPacket(new ObjectUpdateComposer(item, room.OwnerId));

                item.UpdateState();

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `room_items_toner` SET `enabled` = '" + room.TonerData.Enabled + "' LIMIT 1");
                }
                return;
            }

            if (item.Data.InteractionType == InteractionType.GNOME_BOX && item.UserID == session.GetHabbo().Id)
            {
                session.SendPacket(new GnomeBoxComposer(item.Id));
            }

            bool toggle = true;
            if (item.GetBaseItem().InteractionType == InteractionType.WF_FLOOR_SWITCH_1 || item.GetBaseItem().InteractionType == InteractionType.WF_FLOOR_SWITCH_2)
            {
                RoomUser user = item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
                if (user == null)
                    return;

                if (!Gamemap.TilesTouching(item.GetX, item.GetY, user.X, user.Y))
                {
                    toggle = false;
                }
            }

            int request = packet.PopInt();

            item.Interactor.OnTrigger(session, item, request, hasRights);

            if (toggle)
                item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, session.GetHabbo(), item);

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, item.GetBaseItem().Id);
      
        }
    }
}
