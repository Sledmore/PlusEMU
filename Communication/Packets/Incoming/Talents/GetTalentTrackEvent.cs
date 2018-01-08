using System.Collections.Generic;

using Plus.HabboHotel.Talents;
using Plus.Communication.Packets.Outgoing.Talents;

namespace Plus.Communication.Packets.Incoming.Talents
{
    class GetTalentTrackEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            ICollection<TalentTrackLevel> Levels = PlusEnvironment.GetGame().GetTalentTrackManager().GetLevels();

            Session.SendPacket(new TalentTrackComposer(Levels, Type));
        }
    }
}
