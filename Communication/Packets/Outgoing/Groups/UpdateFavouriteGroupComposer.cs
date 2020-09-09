using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class UpdateFavouriteGroupComposer : MessageComposer
    {
        public Group Group { get; }
        public int VirtualId { get; }

        public UpdateFavouriteGroupComposer(Group Group, int VirtualId)
            : base(ServerPacketHeader.UpdateFavouriteGroupMessageComposer)
        {
            this.Group = Group;
            this.VirtualId = VirtualId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);//Sends 0 on .COM
            packet.WriteInteger(Group != null ? Group.Id : 0);
            packet.WriteInteger(3);
            packet.WriteString(Group != null ? Group.Name : string.Empty);
        }
    }
}
