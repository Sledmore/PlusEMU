namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class HabboActivityPointNotificationComposer : ServerPacket
    {
        public HabboActivityPointNotificationComposer(int balance, int notify, int type = 0)
            : base(ServerPacketHeader.HabboActivityPointNotificationMessageComposer)
        {
            WriteInteger(balance);
            WriteInteger(notify);
            WriteInteger(type);
        }
    }
}
