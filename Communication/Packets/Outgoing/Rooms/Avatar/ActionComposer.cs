namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class ActionComposer : MessageComposer
    {
        public int VirtualId { get; }
        public int Action { get; }

        public ActionComposer(int VirtualId, int Action)
            : base(ServerPacketHeader.ActionMessageComposer)
        {
            this.VirtualId = VirtualId;
            this.Action = Action;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Action);
        }
    }
}