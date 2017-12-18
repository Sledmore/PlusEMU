using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class PurchaseGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            string Name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string Description = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            int RoomId = packet.PopInt();
            int Colour1 = packet.PopInt();
            int Colour2 = packet.PopInt();
            int Unknown = packet.PopInt();

            int groupCost = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("catalog.group.purchase.cost"));

            if (session.GetHabbo().Credits < groupCost)
            {
                session.SendPacket(new BroadcastMessageAlertComposer("A group costs " + groupCost + " credits! You only have " + session.GetHabbo().Credits + "!"));
                return;
            }
            else
            {
                session.GetHabbo().Credits -= groupCost;
                session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));
            }

            RoomData Room = null;
            if (!RoomFactory.TryGetData(RoomId, out Room))
                return;
            
            if (Room == null || Room.OwnerId != session.GetHabbo().Id || Room.Group != null)
                return;

            string Badge = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                Badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
            }

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryCreateGroup(session.GetHabbo(), Name, Description, RoomId, Badge, Colour1, Colour2, out Group))
            {
                session.SendNotification("An error occured whilst trying to create this group.\n\nTry again. If you get this message more than once, report it at the link below.\r\rhttp://boonboards.com");
                return;
            }

            session.SendPacket(new PurchaseOKComposer());

            Room.Group = Group;

            if (session.GetHabbo().CurrentRoomId != Room.Id)
                session.SendPacket(new RoomForwardComposer(Room.Id));

            session.SendPacket(new NewGroupInfoComposer(RoomId, Group.Id));
        }
    }
}