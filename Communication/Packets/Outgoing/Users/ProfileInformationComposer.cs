using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class ProfileInformationComposer : ServerPacket
    {
        public ProfileInformationComposer(Habbo habbo, GameClient session, List<Group> groups, int friendCount)
            : base(ServerPacketHeader.ProfileInformationMessageComposer)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(habbo.AccountCreated);

            base.WriteInteger(habbo.Id);
            base.WriteString(habbo.Username);
            base.WriteString(habbo.Look);
            base.WriteString(habbo.Motto);
            base.WriteString(origin.ToString("dd/MM/yyyy"));
            base.WriteInteger(habbo.GetStats().AchievementPoints);
            base.WriteInteger(friendCount); // Friend Count
            base.WriteBoolean(habbo.Id != session.GetHabbo().Id && session.GetHabbo().GetMessenger().FriendshipExists(habbo.Id)); //  Is friend
            base.WriteBoolean(habbo.Id != session.GetHabbo().Id && !session.GetHabbo().GetMessenger().FriendshipExists(habbo.Id) && session.GetHabbo().GetMessenger().RequestExists(habbo.Id)); // Sent friend request
            base.WriteBoolean((PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(habbo.Id)) != null);

            base.WriteInteger(groups.Count);
            foreach (Group group in groups)
            {
                base.WriteInteger(group.Id);
                base.WriteString(group.Name);
                base.WriteString(group.Badge);
                base.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                base.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                base.WriteBoolean(habbo.GetStats().FavouriteGroupId == group.Id); // todo favs
                base.WriteInteger(0);//what the fuck
                base.WriteBoolean(group != null ? group.ForumEnabled : true);//HabboTalk
            }

            base.WriteInteger(Convert.ToInt32(PlusEnvironment.GetUnixTimestamp() - habbo.LastOnline)); // Last online
            base.WriteBoolean(true); // Show the profile
        }
    }
}