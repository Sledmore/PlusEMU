using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;
using Plus.Communication.Packets.Outgoing.Pets;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class RespectPetEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom || session.GetHabbo().GetStats() == null || session.GetHabbo().GetStats().DailyPetRespectPoints == 0)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            RoomUser thisUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (thisUser == null)
                return;

            int petId = packet.PopInt();

            if (!session.GetHabbo().CurrentRoom.GetRoomUserManager().TryGetPet(petId, out RoomUser pet)) 
            {
                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser targetUser = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(petId);
                if (targetUser == null)
                    return;

                //Check some values first, please!
                if (targetUser.GetClient() == null || targetUser.GetClient().GetHabbo() == null)
                    return;

                if (targetUser.GetClient().GetHabbo().Id == session.GetHabbo().Id)
                {
                    session.SendWhisper("Oops, you cannot use this on yourself! (You haven't lost a point, simply reload!)");
                    return;
                }

                //And boom! Let us send some respect points.
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialRespect);
                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RespectGiven", 1);
                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(targetUser.GetClient(), "ACH_RespectEarned", 1);

                //Take away from pet respect points, just in-case users abuse this..
                session.GetHabbo().GetStats().DailyPetRespectPoints -= 1;
                session.GetHabbo().GetStats().RespectGiven += 1;
                targetUser.GetClient().GetHabbo().GetStats().Respect += 1;

                //Apply the effect.
                thisUser.CarryItemId = 999999999;
                thisUser.CarryTimer = 5;

                //Send the magic out.
                if (room.RespectNotificationsEnabled)
                    room.SendPacket(new RespectPetNotificationMessageComposer(targetUser.GetClient().GetHabbo(), targetUser));
                room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemId));
                return;
            }

            if (pet == null || pet.PetData == null || pet.RoomId != session.GetHabbo().CurrentRoomId)
                return;

            session.GetHabbo().GetStats().DailyPetRespectPoints -= 1;
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetRespectGiver", 1);

            thisUser.CarryItemId = 999999999;
            thisUser.CarryTimer = 5;
            pet.PetData.OnRespect();
            room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemId));
        }
    }
}