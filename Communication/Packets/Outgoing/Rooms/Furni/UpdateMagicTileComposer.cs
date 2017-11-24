using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni
{
    class UpdateMagicTileComposer : ServerPacket
    {
        public UpdateMagicTileComposer(int ItemId, int Decimal)
            : base(ServerPacketHeader.UpdateMagicTileMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(ItemId));
            base.WriteInteger(Decimal);
        }
    }
}
