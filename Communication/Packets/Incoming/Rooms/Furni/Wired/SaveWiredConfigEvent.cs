using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;
using Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.Wired
{
    class SaveWiredConfigEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, false, true))
                return;

            int ItemId = Packet.PopInt();

            Session.SendPacket(new HideWiredConfigComposer());

            Item SelectedItem = Room.GetRoomItemHandler().GetItem(ItemId);
            if (SelectedItem == null)
                return;

            IWiredItem Box = null;
            if (!Session.GetHabbo().CurrentRoom.GetWired().TryGet(ItemId, out Box))
                return;

            if (Box.Type == WiredBoxType.EffectGiveUserBadge && !Session.GetHabbo().GetPermissions().HasRight("room_item_wired_rewards"))
            {
                Session.SendNotification("You don't have the correct permissions to do this.");
                return;
            }

            Box.HandleSave(Packet);
            Session.GetHabbo().CurrentRoom.GetWired().SaveBox(Box);
        }
    }
}
