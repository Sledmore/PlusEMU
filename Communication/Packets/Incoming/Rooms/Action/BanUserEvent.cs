using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class BanUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (((Room.WhoCanBan == 0 && !Room.CheckRights(Session, true) && Room.Group == null) || (Room.WhoCanBan == 1 && !Room.CheckRights(Session)) && Room.Group == null) || (Room.Group != null && !Room.CheckRights(Session, false, true)))
                return;

            int UserId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            string R = Packet.PopString();

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToInt32(UserId));
            if (User == null || User.IsBot)
                return;

            if (Room.OwnerId == UserId)
                return;

            if (User.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            long Time = 0;
            if (R.ToLower().Contains("hour"))
                Time = 3600;
            else if (R.ToLower().Contains("day"))
                Time = 86400;
            else if (R.ToLower().Contains("perm"))
                Time = 78892200;

            Room.GetBans().Ban(User, Time);

            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModBanSeen", 1);
        }
    }
}