using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class UpdateFavouriteGroupComposer : ServerPacket
    {
        public UpdateFavouriteGroupComposer(Group Group, int VirtualId)
            : base(ServerPacketHeader.UpdateFavouriteGroupMessageComposer)
        {
            WriteInteger(VirtualId);//Sends 0 on .COM
            WriteInteger(Group != null ? Group.Id : 0);
            WriteInteger(3);
            WriteString(Group != null ? Group.Name : string.Empty);
        }
    }
}
