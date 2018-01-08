using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
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
