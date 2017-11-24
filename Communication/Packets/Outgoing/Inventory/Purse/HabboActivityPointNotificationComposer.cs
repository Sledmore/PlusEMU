using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class HabboActivityPointNotificationComposer : ServerPacket
    {
        public HabboActivityPointNotificationComposer(int Balance, int Notif, int currencyType = 0)
            : base(ServerPacketHeader.HabboActivityPointNotificationMessageComposer)
        {
            base.WriteInteger(Balance);
            base.WriteInteger(Notif);
            base.WriteInteger(currencyType);//Type
        }
    }
}
