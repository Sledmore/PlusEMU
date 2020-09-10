using System;
using System.Collections.Generic;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class ProfileInformationComposer : MessageComposer
    {
        public Habbo TargetHabbo { get; }
        public List<Group> Groups { get; }
        public int FriendCount { get; }
        public Habbo MyHabbo { get; }
        public ProfileInformationComposer(Habbo habbo, GameClient session, List<Group> groups, int friendCount)
            : base(ServerPacketHeader.ProfileInformationMessageComposer)
        {
            this.TargetHabbo = habbo;
            this.Groups = groups;
            this.FriendCount = friendCount;
            this.MyHabbo = session.GetHabbo();
        }

        public override void Compose(ServerPacket packet)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(TargetHabbo.AccountCreated);

            packet.WriteInteger(TargetHabbo.Id);
            packet.WriteString(TargetHabbo.Username);
            packet.WriteString(TargetHabbo.Look);
            packet.WriteString(TargetHabbo.Motto);
            packet.WriteString(origin.ToString("dd/MM/yyyy"));
            packet.WriteInteger(TargetHabbo.GetStats().AchievementPoints);
            packet.WriteInteger(FriendCount); // Friend Count
            packet.WriteBoolean(TargetHabbo.Id != MyHabbo.Id && MyHabbo.GetMessenger().FriendshipExists(TargetHabbo.Id)); //  Is friend
            packet.WriteBoolean(TargetHabbo.Id != MyHabbo.Id && !MyHabbo.GetMessenger().FriendshipExists(TargetHabbo.Id) && MyHabbo.GetMessenger().RequestExists(TargetHabbo.Id)); // Sent friend request
            packet.WriteBoolean((PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(TargetHabbo.Id)) != null);

            packet.WriteInteger(Groups.Count);
            foreach (Group group in Groups)
            {
                packet.WriteInteger(group.Id);
                packet.WriteString(group.Name);
                packet.WriteString(group.Badge);
                packet.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                packet.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                packet.WriteBoolean(TargetHabbo.GetStats().FavouriteGroupId == group.Id); // todo favs
                packet.WriteInteger(0);//what the fuck
                packet.WriteBoolean(group != null ? group.ForumEnabled : true);//HabboTalk
            }

            packet.WriteInteger(Convert.ToInt32(PlusEnvironment.GetUnixTimestamp() - TargetHabbo.LastOnline)); // Last online
            packet.WriteBoolean(true); // Show the profile
        }
    }
}