using System.Diagnostics.Contracts;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class CheckGnomeNameComposer : MessageComposer
    {
        public string PetName { get; }
        public int ErrorId { get; }

        public CheckGnomeNameComposer(string PetName, int ErrorId)
            : base(ServerPacketHeader.CheckGnomeNameMessageComposer)
        {
            this.PetName = PetName;
            this.ErrorId = ErrorId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
            packet.WriteInteger(ErrorId);
            packet.WriteString(PetName);
        }
    }
}
