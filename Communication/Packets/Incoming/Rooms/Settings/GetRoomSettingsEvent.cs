using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            Session.SendPacket(new RoomSettingsDataComposer(Room));
        }
    }
}
