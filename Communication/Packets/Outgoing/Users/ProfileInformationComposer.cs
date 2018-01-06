using System;
using System.Collections.Generic;
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

            WriteInteger(habbo.Id);
            WriteString(habbo.Username);
            WriteString(habbo.Look);
            WriteString(habbo.Motto);
            WriteString(origin.ToString("dd/MM/yyyy"));
            WriteInteger(habbo.GetStats().AchievementPoints);
            WriteInteger(friendCount); // Friend Count
            WriteBoolean(habbo.Id != session.GetHabbo().Id && session.GetHabbo().GetMessenger().FriendshipExists(habbo.Id)); //  Is friend
            WriteBoolean(habbo.Id != session.GetHabbo().Id && !session.GetHabbo().GetMessenger().FriendshipExists(habbo.Id) && session.GetHabbo().GetMessenger().RequestExists(habbo.Id)); // Sent friend request
            WriteBoolean((PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(habbo.Id)) != null);

            WriteInteger(groups.Count);
            foreach (Group group in groups)
            {
                WriteInteger(group.Id);
                WriteString(group.Name);
                WriteString(group.Badge);
                WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                WriteBoolean(habbo.GetStats().FavouriteGroupId == group.Id); // todo favs
                WriteInteger(0);//what the fuck
                WriteBoolean(group != null ? group.ForumEnabled : true);//HabboTalk
            }

            WriteInteger(Convert.ToInt32(PlusEnvironment.GetUnixTimestamp() - habbo.LastOnline)); // Last online
            WriteBoolean(true); // Show the profile
        }
    }
}