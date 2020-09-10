using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class NameChangeUpdateComposer : MessageComposer
    {
        public int Error { get; }
        public string Name { get; }
        public ICollection<string> Tags { get; }
        public NameChangeUpdateComposer(string name, int error, ICollection<string> tags)
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {
            this.Error = error;
            this.Name = name;
            this.Tags = tags;
        }

        public NameChangeUpdateComposer(string name, int error)
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {
            this.Error = error;
            this.Name = name;
            this.Tags = new List<string>();
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error);
            packet.WriteString(Name);

            packet.WriteInteger(Tags.Count);
            foreach (string tag in Tags)
            {
                packet.WriteString(Name + tag);
            }
        }
    }
}
