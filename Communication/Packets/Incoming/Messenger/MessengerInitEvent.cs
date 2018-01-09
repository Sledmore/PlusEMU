using System.Linq;
using System.Collections.Generic;

using MoreLinq;
using Plus.HabboHotel.Users.Messenger;
using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class MessengerInitEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null)
                return;

            session.GetHabbo().GetMessenger().OnStatusChanged(false);

            ICollection<MessengerBuddy> friends = new List<MessengerBuddy>();
            foreach (MessengerBuddy buddy in session.GetHabbo().GetMessenger().GetFriends().ToList())
            {
                if (buddy == null || buddy.IsOnline)
                    continue;

                friends.Add(buddy);
            }

            session.SendPacket(new MessengerInitComposer());

            int page = 0;
            if (!friends.Any())
            {
                session.SendPacket(new BuddyListComposer(friends, session.GetHabbo(), 1, 0));
            }
            else
            {
                int pages = (friends.Count() - 1) / 500 + 1;
                foreach (ICollection<MessengerBuddy> batch in friends.Batch(500))
                {
                    session.SendPacket(new BuddyListComposer(batch.ToList(), session.GetHabbo(), pages, page));

                    page++;
                }
            }
          
            session.GetHabbo().GetMessenger().ProcessOfflineMessages();
        }
    }
}