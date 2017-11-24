using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class CloseIssueDefaultActionEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;
        }
    }
}
