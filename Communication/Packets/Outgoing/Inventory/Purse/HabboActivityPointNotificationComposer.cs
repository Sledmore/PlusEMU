namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class HabboActivityPointNotificationComposer : ServerPacket
    {
        public HabboActivityPointNotificationComposer(int balance, int notify, int type = 0)
            : base(ServerPacketHeader.HabboActivityPointNotificationMessageComposer)
        {
            base.WriteInteger(balance);
            base.WriteInteger(notify);
            base.WriteInteger(type);
        }
    }
}
