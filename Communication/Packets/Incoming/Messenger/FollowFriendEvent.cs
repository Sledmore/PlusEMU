using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class FollowFriendEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int BuddyId = Packet.PopInt();
            if (BuddyId == 0 || BuddyId == Session.GetHabbo().Id)
                return;

            GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(BuddyId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            if (!Client.GetHabbo().InRoom)
            {
                Session.SendPacket(new FollowFriendFailedComposer(2));
                Session.GetHabbo().GetMessenger().UpdateFriend(Client.GetHabbo().Id, Client, true);
                return;
            }
            else if (Session.GetHabbo().CurrentRoom != null && Client.GetHabbo().CurrentRoom != null)
            {
                if (Session.GetHabbo().CurrentRoom.RoomId == Client.GetHabbo().CurrentRoom.RoomId)
                    return;
            }

            Session.SendPacket(new RoomForwardComposer(Client.GetHabbo().CurrentRoomId));
        }
    }
}
