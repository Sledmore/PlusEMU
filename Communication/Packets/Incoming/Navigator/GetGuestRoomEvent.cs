using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class GetGuestRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomID = Packet.PopInt();

            RoomData roomData = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (roomData == null)
                return;

            Boolean isLoading = Packet.PopInt() == 1;
            Boolean checkEntry = Packet.PopInt() == 1;

            Session.SendPacket(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));
        }
    }
}
