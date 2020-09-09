namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class FloodControlComposer : MessageComposer
    {
        public int FloodTime { get; }

        public FloodControlComposer(int floodTime)
            : base(ServerPacketHeader.FloodControlMessageComposer)
        {
            this.FloodTime = floodTime;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(FloodTime);
        }
    }
}