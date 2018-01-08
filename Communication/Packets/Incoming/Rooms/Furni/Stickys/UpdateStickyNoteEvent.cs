using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.Stickys
{
    class UpdateStickyNoteEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            Item item = room.GetRoomItemHandler().GetItem(packet.PopInt());
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.POSTIT)
                return;

            string color = packet.PopString();
            string text = packet.PopString();

            if (!room.CheckRights(session))
            {
                if (!text.StartsWith(item.ExtraData))
                    return; // we can only ADD stuff! older stuff changed, this is not allowed
            }

            switch (color)
            {
                case "FFFF33":
                case "FF9CFF":
                case "9CCEFF":
                case "9CFF9C":

                    break;

                default:

                    return; // invalid color
            }

            item.ExtraData = color + " " + text;
            item.UpdateState(true, true);
        }
    }
}
