using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class UpdateNavigatorSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomID = Packet.PopInt();
            if (roomID == 0)
                return;

            RoomData Data = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (Data == null)
                return;

            Session.GetHabbo().HomeRoom = roomID;
            Session.SendPacket(new NavigatorSettingsComposer(roomID));
        }
    }
}
