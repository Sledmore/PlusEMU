using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.Communication.Packets.Incoming.Rooms.FloorPlan
{
    class FloorPlanEditorRoomPropertiesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            DynamicRoomModel Model = Room.GetGameMap().Model;
            if (Model == null)
                return;

            ICollection<Item> FloorItems = Room.GetRoomItemHandler().GetFloor;

            Session.SendPacket(new FloorPlanFloorMapComposer(FloorItems));
            Session.SendPacket(new FloorPlanSendDoorComposer(Model.DoorX, Model.DoorY, Model.DoorOrientation));
            Session.SendPacket(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, PlusEnvironment.EnumToBool(Room.Hidewall.ToString())));
        }
    }
}
