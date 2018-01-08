using System;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class PurchaseGroupEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            string name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string description = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            int roomId = packet.PopInt();
            int mainColour = packet.PopInt();
            int secondaryColour = packet.PopInt();
            packet.PopInt(); //unknown

            int groupCost = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("catalog.group.purchase.cost"));

            if (session.GetHabbo().Credits < groupCost)
            {
                session.SendPacket(new BroadcastMessageAlertComposer("A group costs " + groupCost + " credits! You only have " + session.GetHabbo().Credits + "!"));
                return;
            }

            session.GetHabbo().Credits -= groupCost;
            session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));

            if (!RoomFactory.TryGetData(roomId, out RoomData room))
                return;

            if (room == null || room.OwnerId != session.GetHabbo().Id || room.Group != null)
                return;

            string badge = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
            }

            if (!PlusEnvironment.GetGame().GetGroupManager().TryCreateGroup(session.GetHabbo(), name, description, roomId, badge, mainColour, secondaryColour, out Group group))
            {
                session.SendNotification("An error occured whilst trying to create this group.\n\nTry again. If you get this message more than once, report it at the link below.\r\rhttp://boonboards.com");
                return;
            }

            session.SendPacket(new PurchaseOKComposer());

            room.Group = group;

            if (session.GetHabbo().CurrentRoomId != room.Id)
                session.SendPacket(new RoomForwardComposer(room.Id));

            session.SendPacket(new NewGroupInfoComposer(roomId, group.Id));
        }
    }
}