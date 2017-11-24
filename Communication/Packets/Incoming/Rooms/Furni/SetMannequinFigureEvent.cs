using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;


namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class SetMannequinFigureEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            int ItemId = Packet.PopInt();
            Item Item = Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            string Gender = Session.GetHabbo().Gender.ToLower();
            string Figure = "";

            foreach (string Str in Session.GetHabbo().Look.Split('.'))
            {
                if (Str.Contains("hr") || Str.Contains("hd") || Str.Contains("he") || Str.Contains("ea") || Str.Contains("ha"))
                    continue;

                Figure += Str + ".";
            }

            Figure = Figure.TrimEnd('.');
            if (Item.ExtraData.Contains(Convert.ToChar(5)))
            {
                string[] Flags = Item.ExtraData.Split(Convert.ToChar(5));
                Item.ExtraData = Gender + Convert.ToChar(5) + Figure + Convert.ToChar(5) + Flags[2];
            }
            else
                Item.ExtraData = Gender + Convert.ToChar(5) + Figure + Convert.ToChar(5) + "Default";

            Item.UpdateState(true, true);
        }
    }
}
