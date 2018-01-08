using System;
using System.Linq;
using System.Collections.Generic;
using Plus.HabboHotel.Items.Televisions;
using Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
    class YouTubeGetNextVideo : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            ICollection<TelevisionItem> videos = PlusEnvironment.GetGame().GetTelevisionManager().TelevisionList;

            if (videos.Count == 0)
            {
                session.SendNotification("Oh, it looks like the hotel manager haven't added any videos for you to watch! :(");
                return;
            }

            int itemId = packet.PopInt();
            packet.PopInt(); //next

            TelevisionItem item = null;
            Dictionary<int, TelevisionItem> dict = PlusEnvironment.GetGame().GetTelevisionManager()._televisions;
            foreach (TelevisionItem value in RandomValues(dict).Take(1))
            {
                item = value;
            }

            if(item == null)
            {
                session.SendNotification("Oh, it looks like their was a problem getting the video.");
                return;
            }

            session.SendPacket(new GetYouTubeVideoComposer(itemId, item.YouTubeId));
        }

        private static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            Random rand = new Random();
            List<TValue> values = dict.Values.ToList();
            int size = dict.Count;
            while (true)
            {
                yield return values[rand.Next(size)];
            }
        }
    }
}