using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class ActionComposer : ServerPacket
    {
        public ActionComposer(int VirtualId, int Action)
            : base(ServerPacketHeader.ActionMessageComposer)
        {
            base.WriteInteger(VirtualId);
            base.WriteInteger(Action);
        }
    }
}