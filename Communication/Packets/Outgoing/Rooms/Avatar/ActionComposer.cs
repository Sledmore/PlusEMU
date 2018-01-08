namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class ActionComposer : ServerPacket
    {
        public ActionComposer(int VirtualId, int Action)
            : base(ServerPacketHeader.ActionMessageComposer)
        {
            WriteInteger(VirtualId);
            WriteInteger(Action);
        }
    }
}