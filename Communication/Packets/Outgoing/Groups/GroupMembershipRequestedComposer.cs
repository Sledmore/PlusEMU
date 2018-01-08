using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupMembershipRequestedComposer : ServerPacket
    {
        public GroupMembershipRequestedComposer(int GroupId, Habbo Habbo, int Type) 
            : base(ServerPacketHeader.GroupMembershipRequestedMessageComposer)
        {
            WriteInteger(GroupId);//GroupId
            WriteInteger(Type);//Type?
            {
                WriteInteger(Habbo.Id);//UserId
                WriteString(Habbo.Username);
                WriteString(Habbo.Look);
                WriteString(string.Empty);
            }
        }
    }
}
