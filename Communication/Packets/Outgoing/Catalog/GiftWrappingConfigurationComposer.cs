namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class GiftWrappingConfigurationComposer : ServerPacket
    {
        public GiftWrappingConfigurationComposer()
            : base(ServerPacketHeader.GiftWrappingConfigurationMessageComposer)
        {
            WriteBoolean(true);
            WriteInteger(1);
            WriteInteger(10);
            for (int i = 3372; i < 3382;)
            {
                WriteInteger(i);
                i++;
            }
            WriteInteger(7);
            WriteInteger(0);
            WriteInteger(1);
            WriteInteger(2);
            WriteInteger(3);
            WriteInteger(4);
            WriteInteger(5);
            WriteInteger(6);
            WriteInteger(11);
            WriteInteger(0);
            WriteInteger(1);
            WriteInteger(2);
            WriteInteger(3);
            WriteInteger(4);
            WriteInteger(5);
            WriteInteger(6);
            WriteInteger(7);
            WriteInteger(8);
            WriteInteger(9);
            WriteInteger(10);
            WriteInteger(7);
            for (int i = 187; i < 194;)
            {
                WriteInteger(i);
                i++;
            }
        }
    }
}