namespace Plus.Communication.Packets.Outgoing.Availability
{
    class MaintenanceStatusComposer : ServerPacket
    {
        public MaintenanceStatusComposer(int minutes, int duration)
            : base(ServerPacketHeader.MaintenanceStatusMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(minutes);//Time till shutdown.
            base.WriteInteger(duration);//Duration
        }
    }
}