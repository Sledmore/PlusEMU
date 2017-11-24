using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Messenger;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class FindNewFriendsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Instance = PlusEnvironment.GetGame().GetRoomManager().TryGetRandomLoadedRoom();

            if (Instance != null)
            {
                Session.SendPacket(new FindFriendsProcessResultComposer(true));
                Session.SendPacket(new RoomForwardComposer(Instance.Id));
            }
            else
            {
                Session.SendPacket(new FindFriendsProcessResultComposer(false));
            }
        }
    }
}
