using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class HabboSearchResultComposer : ServerPacket
    {
        public HabboSearchResultComposer(List<SearchResult> Friends, List<SearchResult> OtherUsers)
            : base(ServerPacketHeader.HabboSearchResultMessageComposer)
        {
            base.WriteInteger(Friends.Count);
            foreach (SearchResult Friend in Friends.ToList())
            {
                bool Online = (PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Friend.UserId) != null);

                base.WriteInteger(Friend.UserId);
               base.WriteString(Friend.Username);
               base.WriteString(Friend.Motto);
                base.WriteBoolean(Online);
                base.WriteBoolean(false);
               base.WriteString(string.Empty);
                base.WriteInteger(0);
               base.WriteString(Online ? Friend.Figure : "");
               base.WriteString(Friend.LastOnline);
            }

            base.WriteInteger(OtherUsers.Count);
            foreach (SearchResult OtherUser in OtherUsers.ToList())
            {
                bool Online = (PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(OtherUser.UserId) != null);

                base.WriteInteger(OtherUser.UserId);
               base.WriteString(OtherUser.Username);
               base.WriteString(OtherUser.Motto);
                base.WriteBoolean(Online);
                base.WriteBoolean(false);
               base.WriteString(string.Empty);
                base.WriteInteger(0);
               base.WriteString(Online ? OtherUser.Figure : "");
               base.WriteString(OtherUser.LastOnline);
            }
        }
    }
}
