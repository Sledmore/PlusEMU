using System.Collections.Generic;
using Plus.HabboHotel.Rooms;
namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PromotableRoomsComposer : MessageComposer
    {
        public ICollection<RoomData> Rooms { get; }

        public PromotableRoomsComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.PromotableRoomsMessageComposer)
        {
            this.Rooms = Rooms;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(true);
            packet.WriteInteger(Rooms.Count);//Count

            foreach (RoomData Data in Rooms)
            {
                packet.WriteInteger(Data.Id);
                packet.WriteString(Data.Name);
                packet.WriteBoolean(false);
            }
        }
    }
}