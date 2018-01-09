using System;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class GiveHandItemEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;            

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            RoomUser targetUser = room.GetRoomUserManager().GetRoomUserByHabbo(packet.PopInt());
            if (targetUser == null)
                return;

            if (!(Math.Abs(user.X - targetUser.X) >= 3 || Math.Abs(user.Y - targetUser.Y) >= 3) || session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                if (user.CarryItemId > 0 && user.CarryTimer > 0)
                {
                    if (user.CarryItemId == 8)
                        PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.GiveCoffee);
                    targetUser.CarryItem(user.CarryItemId);
                    user.CarryItem(0);
                    targetUser.DanceId = 0;
                }
            }
        }
    }
}
