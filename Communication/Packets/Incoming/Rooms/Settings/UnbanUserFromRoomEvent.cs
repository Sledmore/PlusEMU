using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class UnbanUserFromRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room Instance = session.GetHabbo().CurrentRoom;
            if (Instance == null || !Instance.CheckRights(session, true))
                return;

            int UserId = packet.PopInt();
            int RoomId = packet.PopInt();

            if (Instance.GetBans().IsBanned(UserId))
            {
                Instance.GetBans().Unban(UserId);

                session.SendPacket(new UnbanUserFromRoomComposer(RoomId, UserId));
            }
        }
    }
}