using System;
using System.Linq;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Relationships;
using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class FriendListUpdateComposer : ServerPacket
    {
        public FriendListUpdateComposer(int FriendId)
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
            WriteInteger(0);//Category Count
            WriteInteger(1);//Updates Count
            WriteInteger(-1);//Update
            WriteInteger(FriendId);
        }

        public FriendListUpdateComposer(GameClient Session, MessengerBuddy Buddy)
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
            WriteInteger(0);//Category Count
            WriteInteger(1);//Updates Count
            WriteInteger(0);//Update

            Relationship Relationship = Session.GetHabbo().Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Buddy.UserId)).Value;
            int y = Relationship == null ? 0 : Relationship.Type;

            WriteInteger(Buddy.UserId);
           WriteString(Buddy.mUsername);
            WriteInteger(1);
            if (!Buddy.mAppearOffline || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                WriteBoolean(Buddy.IsOnline);
            else
                WriteBoolean(false);

            if (!Buddy.mHideInroom || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                WriteBoolean(Buddy.InRoom);
            else
                WriteBoolean(false);

           WriteString("");//Habbo.IsOnline ? Habbo.Look : "");
            WriteInteger(0); // categoryid
           WriteString(Buddy.mMotto);
           WriteString(string.Empty); // Facebook username
           WriteString(string.Empty);
            WriteBoolean(true); // Allows offline messaging
            WriteBoolean(false); // ?
            WriteBoolean(false); // Uses phone
            WriteShort(y);
        }
    }
}
