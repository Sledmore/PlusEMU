using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class NameChangeUpdateComposer : ServerPacket
    {
        public NameChangeUpdateComposer(string name, int error, ICollection<string> tags)
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {
            WriteInteger(error);
            WriteString(name);

            WriteInteger(tags.Count);
            foreach (string tag in tags)
            {
                WriteString(name + tag);
            }
        }

        public NameChangeUpdateComposer(string name, int error)
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {
            WriteInteger(error);
            WriteString(name);
            WriteInteger(0);
        }
    }
}
