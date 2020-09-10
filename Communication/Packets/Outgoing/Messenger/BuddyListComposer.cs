using System;
using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Relationships;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class BuddyListComposer : MessageComposer
    {
        public ICollection<MessengerBuddy> Friends { get; }
        public Habbo Habbo { get; }
        public int Pages { get; }
        public int Page { get; }

        public BuddyListComposer(ICollection<MessengerBuddy> friends, Habbo player, int pages, int page)
            : base(ServerPacketHeader.BuddyListMessageComposer)
        {
            this.Friends = friends;
            this.Pages = pages;
            this.Habbo = player;
            this.Page = page;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Pages);// Pages
            packet.WriteInteger(Page);// Page

            packet.WriteInteger(Friends.Count);
            foreach (MessengerBuddy Friend in Friends.ToList())
            {
                Relationship Relationship = Habbo.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Friend.UserId)).Value;

                packet.WriteInteger(Friend.Id);
                packet.WriteString(Friend.mUsername);
                packet.WriteInteger(1);//Gender.
                packet.WriteBoolean(Friend.IsOnline);
                packet.WriteBoolean(Friend.IsOnline && Friend.InRoom);
                packet.WriteString(Friend.IsOnline ? Friend.mLook : string.Empty);
                packet.WriteInteger(0); // category id
                packet.WriteString(Friend.IsOnline ? Friend.mMotto : string.Empty);
                packet.WriteString(string.Empty);//Alternative name?
                packet.WriteString(string.Empty);
                packet.WriteBoolean(true);
                packet.WriteBoolean(false);
                packet.WriteBoolean(false);//Pocket Habbo user.
                packet.WriteShort(Relationship == null ? 0 : Relationship.Type);
            }
        }
    }
}
