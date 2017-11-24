using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.LandingView
{
    class CampaignComposer : ServerPacket
    {
        public CampaignComposer(string campaignString, string campaignName)
            : base(ServerPacketHeader.CampaignMessageComposer)
        {
           base.WriteString(campaignString);
           base.WriteString(campaignName);
        }
    }
}
