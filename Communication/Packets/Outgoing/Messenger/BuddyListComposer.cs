using System;
using System.Linq;
using System.Text;
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
            base.WriteInteger(pages);// Pages
            base.WriteInteger(page);// Page

            base.WriteInteger(friends.Count);
            foreach (MessengerBuddy Friend in friends.ToList())
            {
                Relationship Relationship = player.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Friend.UserId)).Value;

                base.WriteInteger(Friend.Id);
                base.WriteString(Friend.mUsername);
                base.WriteInteger(1);//Gender.
                base.WriteBoolean(Friend.IsOnline);
                base.WriteBoolean(Friend.IsOnline && Friend.InRoom);
                base.WriteString(Friend.IsOnline ? Friend.mLook : string.Empty);
                base.WriteInteger(0); // category id
                base.WriteString(Friend.IsOnline ? Friend.mMotto : string.Empty);
                base.WriteString(string.Empty);//Alternative name?
                base.WriteString(string.Empty);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteBoolean(false);//Pocket Habbo user.
                base.WriteShort(Relationship == null ? 0 : Relationship.Type);
            }
        }
    }
}
