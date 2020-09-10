namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class HabboActivityPointNotificationComposer : MessageComposer
    {
        public int Balance { get; }
        public int Notify { get; }
        public int Type { get; }

        public HabboActivityPointNotificationComposer(int balance, int notify, int type = 0)
            : base(ServerPacketHeader.HabboActivityPointNotificationMessageComposer)
        {
            this.Balance = balance;
            this.Notify = notify;
            this.Type = type;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Balance);
            packet.WriteInteger(Notify);
            packet.WriteInteger(Type);
        }
    }
}
