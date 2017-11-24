using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class LetUserInEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room Room;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session))
                return;

            string Name = Packet.PopString();
            bool Accepted = Packet.PopBoolean();

            GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Name);
            if (Client == null)
                return;

            if (Accepted)
            {
                Client.GetHabbo().RoomAuthOk = true;
                Client.SendPacket(new FlatAccessibleComposer(""));
                Room.SendPacket(new FlatAccessibleComposer(Client.GetHabbo().Username), true);
            }
            else
            {
                Client.SendPacket(new FlatAccessDeniedComposer(""));
                Room.SendPacket(new FlatAccessDeniedComposer(Client.GetHabbo().Username), true);
            }
        }
    }
}
