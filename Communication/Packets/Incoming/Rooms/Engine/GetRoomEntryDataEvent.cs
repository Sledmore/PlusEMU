using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items.Wired;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class GetRoomEntryDataEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            if (!room.GetRoomUserManager().AddAvatarToRoom(session))
            {
                room.GetRoomUserManager().RemoveUserFromRoom(session, false);
                return;//TODO: Remove?
            }

            room.SendObjects(session);

            if (session.GetHabbo().GetMessenger() != null)
                session.GetHabbo().GetMessenger().OnStatusChanged(true);

            if (session.GetHabbo().GetStats().QuestId > 0)
                PlusEnvironment.GetGame().GetQuestManager().QuestReminder(session, session.GetHabbo().GetStats().QuestId);

            session.SendPacket(new RoomEntryInfoComposer(room.RoomId, room.CheckRights(session, true)));
            session.SendPacket(new RoomVisualizationSettingsComposer(room.WallThickness, room.FloorThickness, PlusEnvironment.EnumToBool(room.Hidewall.ToString())));

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Username);
            if (user != null && session.GetHabbo().PetId == 0)
            {
                room.SendPacket(new UserChangeComposer(user, false));
            }

            session.SendPacket(new RoomEventComposer(room, room.Promotion));

            if (room.GetWired() != null)
                room.GetWired().TriggerEvent(WiredBoxType.TriggerRoomEnter, session.GetHabbo());

            if (PlusEnvironment.GetUnixTimestamp() < session.GetHabbo().FloodTime && session.GetHabbo().FloodTime != 0)
                session.SendPacket(new FloodControlComposer((int)session.GetHabbo().FloodTime - (int)PlusEnvironment.GetUnixTimestamp()));
        }
    }
}