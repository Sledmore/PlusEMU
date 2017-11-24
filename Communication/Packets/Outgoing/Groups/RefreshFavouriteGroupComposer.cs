using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class RefreshFavouriteGroupComposer : ServerPacket
    {
        public RefreshFavouriteGroupComposer(int Id)
            : base(ServerPacketHeader.RefreshFavouriteGroupMessageComposer)
        {
            base.WriteInteger(Id);
        }
    }
}
