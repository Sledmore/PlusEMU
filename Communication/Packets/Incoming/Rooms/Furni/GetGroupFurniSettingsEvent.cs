using Plus.HabboHotel.Items;
using Plus.HabboHotel.Groups;

using Plus.Communication.Packets.Outgoing.Groups;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class GetGroupFurniSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            int ItemId = Packet.PopInt();
            int GroupId = Packet.PopInt();

            Item Item = Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Item.Data.InteractionType != InteractionType.GUILD_GATE)
                return;

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            Session.SendPacket(new GroupFurniSettingsComposer(Group, ItemId, Session.GetHabbo().Id));
            Session.SendPacket(new GroupInfoComposer(Group, Session, false));
        }
    }
}