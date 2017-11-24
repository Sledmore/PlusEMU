using Plus.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Rooms.Avatar
{
    class ApplySignEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int SignId = Packet.PopInt();
            Room Room;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;


            User.UnIdle();

            User.SetStatus("sign", Convert.ToString(SignId));
            User.UpdateNeeded = true;
            User.SignTime = PlusEnvironment.GetUnixTimestamp() + 5;
        }
    }
}