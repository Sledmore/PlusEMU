using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Users
{
    public class IgnoredUsersComposer : MessageComposer
    {
        public IReadOnlyCollection<string> IgnoredUsers { get; }
        public IgnoredUsersComposer(IReadOnlyCollection<string> ignoredUsers)
            : base(ServerPacketHeader.IgnoredUsersMessageComposer)
        {
            this.IgnoredUsers = ignoredUsers;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(IgnoredUsers.Count);
            foreach (string username in IgnoredUsers)
            {
                packet.WriteString(username);
            }
        }
    }
}
