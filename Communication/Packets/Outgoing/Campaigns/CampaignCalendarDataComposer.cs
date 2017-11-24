using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Campaigns
{
    class CampaignCalendarDataComposer : ServerPacket
    {
        public CampaignCalendarDataComposer(List<int> OpenedBoxes, List<int> LateBoxes)
            : base(ServerPacketHeader.CampaignCalendarDataMessageComposer)
        {
            base.WriteString("xmas15");//Set the campaign.
            base.WriteString("");//No idea.
            base.WriteInteger( DateTime.Now.Day - 1);//Start
            base.WriteInteger(25);//End?

            //Opened boxes
            base.WriteInteger(OpenedBoxes.Count);
            foreach (int Day in OpenedBoxes)
            {
                base.WriteInteger(Day);
            }

            //Late boxes?
            base.WriteInteger(LateBoxes.Count);
            foreach (int Day in LateBoxes)
            {
                base.WriteInteger(Day);
            }
        }
    }
}