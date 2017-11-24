using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class SlideObjectBundleComposer : ServerPacket
    {
        public SlideObjectBundleComposer(int FromX, int FromY, double FromZ, int ToX, int ToY, double ToZ, int RollerId, int AvatarId, int ItemId)
            : base(ServerPacketHeader.SlideObjectBundleMessageComposer)
        {
            bool IsItem = ItemId > 0;

            base.WriteInteger(FromX);
            base.WriteInteger(FromY);
            base.WriteInteger(ToX);
            base.WriteInteger(ToY);
            base.WriteInteger(IsItem ? 1 : 0);

            if (IsItem)
                base.WriteInteger(ItemId);
            else
            {
                base.WriteInteger(RollerId);
                base.WriteInteger(2);
                base.WriteInteger(AvatarId);
            }

            base.WriteDouble(FromZ);
            base.WriteDouble(ToZ);

            if (IsItem)
            {
                base.WriteInteger(RollerId);
            }
        }
    }
}
