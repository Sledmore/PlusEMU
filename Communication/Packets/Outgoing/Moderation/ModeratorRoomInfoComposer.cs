using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomInfoComposer : ServerPacket
    {
        public ModeratorRoomInfoComposer(RoomData Data, bool OwnerInRoom)
            : base(ServerPacketHeader.ModeratorRoomInfoMessageComposer)
        {
            WriteInteger(Data.Id);
            WriteInteger(Data.UsersNow);
            WriteBoolean(OwnerInRoom); // owner in room
            WriteInteger(Data.OwnerId);
           WriteString(Data.OwnerName);
            WriteBoolean(Data != null);
           WriteString(Data.Name);
           WriteString(Data.Description);
           
            WriteInteger(Data.Tags.Count);
            foreach (string Tag in Data.Tags)
            {
               WriteString(Tag);
            }

            WriteBoolean(false);
        }
    }
}
