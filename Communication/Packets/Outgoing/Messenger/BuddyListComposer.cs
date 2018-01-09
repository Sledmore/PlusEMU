using System;
using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Relationships;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class BuddyListComposer : ServerPacket
    {
        public BuddyListComposer(ICollection<MessengerBuddy> friends, Habbo player, int pages, int page)
            : base(ServerPacketHeader.BuddyListMessageComposer)
        {
            WriteInteger(pages);// Pages
            WriteInteger(page);// Page

            WriteInteger(friends.Count);
            foreach (MessengerBuddy Friend in friends.ToList())
            {
                Relationship Relationship = player.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Friend.UserId)).Value;

                WriteInteger(Friend.Id);
                WriteString(Friend.mUsername);
                WriteInteger(1);//Gender.
                WriteBoolean(Friend.IsOnline);
                WriteBoolean(Friend.IsOnline && Friend.InRoom);
                WriteString(Friend.IsOnline ? Friend.mLook : string.Empty);
                WriteInteger(0); // category id
                WriteString(Friend.IsOnline ? Friend.mMotto : string.Empty);
                WriteString(string.Empty);//Alternative name?
                WriteString(string.Empty);
                WriteBoolean(true);
                WriteBoolean(false);
                WriteBoolean(false);//Pocket Habbo user.
                WriteShort(Relationship == null ? 0 : Relationship.Type);
            }
        }
    }
}
