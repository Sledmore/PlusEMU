using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Items.Televisions;
using Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
    class YouTubeVideoInformationEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string VideoId = Packet.PopString();

            foreach (TelevisionItem Tele in PlusEnvironment.GetGame().GetTelevisionManager().TelevisionList.ToList())
            {
                if (Tele.YouTubeId != VideoId)
                    continue;

                Session.SendPacket(new GetYouTubeVideoComposer(ItemId, Tele.YouTubeId));
            }
        }
    }
}