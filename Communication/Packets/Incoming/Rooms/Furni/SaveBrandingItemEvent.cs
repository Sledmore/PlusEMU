using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class SaveBrandingItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (!Room.CheckRights(Session, true) || !Session.GetHabbo().GetPermissions().HasRight("room_item_save_branding_items"))
                return;

            int ItemId = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Item.Data.InteractionType == InteractionType.BACKGROUND)
            {
                int Data = Packet.PopInt();
                string BrandData = "state" + Convert.ToChar(9) + "0";

                for (int i = 1; i <= Data; i++)
                {
                    BrandData = BrandData + Convert.ToChar(9) + Packet.PopString();
                }

                Item.ExtraData = BrandData;
            }
            else if (Item.Data.InteractionType == InteractionType.FX_PROVIDER)
            {
                /*int Unknown = Packet.PopInt();
                string Data = Packet.PopString();
                int EffectId = Packet.PopInt();

                Item.ExtraData = Convert.ToString(EffectId);*/
            }

            Room.GetRoomItemHandler().SetFloorItem(Session, Item, Item.GetX, Item.GetY, Item.Rotation, false, false, true);
        }
    }
}