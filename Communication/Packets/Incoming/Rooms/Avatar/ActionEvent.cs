using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Avatar
{
    public class ActionEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            int action = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            if (user.DanceId > 0)
                user.DanceId = 0;

            if (session.GetHabbo().Effects().CurrentEffect > 0)
                room.SendPacket(new AvatarEffectComposer(user.VirtualId, 0));

            user.UnIdle();
            room.SendPacket(new ActionComposer(user.VirtualId, action));

            if (action == 5) // idle
            {
                user.IsAsleep = true;
                room.SendPacket(new SleepComposer(user, true));
            }

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialWave);
        }
    }
}