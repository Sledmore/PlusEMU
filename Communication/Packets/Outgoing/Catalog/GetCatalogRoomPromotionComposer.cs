using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class GetCatalogRoomPromotionComposer : MessageComposer
    {
        public List<RoomData> UsersRooms { get; }

        public GetCatalogRoomPromotionComposer(List<RoomData> UsersRooms)
            : base(ServerPacketHeader.PromotableRoomsMessageComposer)
        {
            this.UsersRooms = UsersRooms;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(true);//wat
            packet.WriteInteger(UsersRooms.Count);//Count of rooms?
            foreach (RoomData Room in UsersRooms)
            {
                packet.WriteInteger(Room.Id);
                packet.WriteString(Room.Name);
                packet.WriteBoolean(true);
            }
        }
    }
}
