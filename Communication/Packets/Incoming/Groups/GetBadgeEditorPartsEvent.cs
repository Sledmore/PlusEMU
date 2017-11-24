using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Groups;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            session.SendPacket(new BadgeEditorPartsComposer(
                PlusEnvironment.GetGame().GetGroupManager().BadgeBases,
                PlusEnvironment.GetGame().GetGroupManager().BadgeSymbols,
                PlusEnvironment.GetGame().GetGroupManager().BadgeBaseColours,
                PlusEnvironment.GetGame().GetGroupManager().BadgeSymbolColours,
                PlusEnvironment.GetGame().GetGroupManager().BadgeBackColours));
        }
    }
}
