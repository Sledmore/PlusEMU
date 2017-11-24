namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class GiftWrappingConfigurationComposer : ServerPacket
    {
        public GiftWrappingConfigurationComposer()
            : base(ServerPacketHeader.GiftWrappingConfigurationMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(1);
            base.WriteInteger(10);
            for (int i = 3372; i < 3382;)
            {
                base.WriteInteger(i);
                i++;
            }
            base.WriteInteger(7);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(2);
            base.WriteInteger(3);
            base.WriteInteger(4);
            base.WriteInteger(5);
            base.WriteInteger(6);
            base.WriteInteger(11);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(2);
            base.WriteInteger(3);
            base.WriteInteger(4);
            base.WriteInteger(5);
            base.WriteInteger(6);
            base.WriteInteger(7);
            base.WriteInteger(8);
            base.WriteInteger(9);
            base.WriteInteger(10);
            base.WriteInteger(7);
            for (int i = 187; i < 194;)
            {
                base.WriteInteger(i);
                i++;
            }
        }
    }
}