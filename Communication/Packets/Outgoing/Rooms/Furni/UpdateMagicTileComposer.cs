using System;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni
{
    class UpdateMagicTileComposer : MessageComposer
    {
        public int ItemId { get; }
        public int Decimal { get; }

        public UpdateMagicTileComposer(int ItemId, int Decimal)
            : base(ServerPacketHeader.UpdateMagicTileMessageComposer)
        {
            this.ItemId = ItemId;
            this.Decimal = Decimal;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteInteger(Decimal);
        }
    }
}
