using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class HabboSearchResultComposer : ServerPacket
    {
        public HabboSearchResultComposer(List<SearchResult> Friends, List<SearchResult> OtherUsers)
            : base(ServerPacketHeader.HabboSearchResultMessageComposer)
        {
            WriteInteger(Friends.Count);
            foreach (SearchResult Friend in Friends.ToList())
            {
                bool Online = (PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Friend.UserId) != null);

                WriteInteger(Friend.UserId);
               WriteString(Friend.Username);
               WriteString(Friend.Motto);
                WriteBoolean(Online);
                WriteBoolean(false);
               WriteString(string.Empty);
                WriteInteger(0);
               WriteString(Online ? Friend.Figure : "");
               WriteString(Friend.LastOnline);
            }

            WriteInteger(OtherUsers.Count);
            foreach (SearchResult OtherUser in OtherUsers.ToList())
            {
                bool Online = (PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(OtherUser.UserId) != null);

                WriteInteger(OtherUser.UserId);
               WriteString(OtherUser.Username);
               WriteString(OtherUser.Motto);
                WriteBoolean(Online);
                WriteBoolean(false);
               WriteString(string.Empty);
                WriteInteger(0);
               WriteString(Online ? OtherUser.Figure : "");
               WriteString(OtherUser.LastOnline);
            }
        }
    }
}
