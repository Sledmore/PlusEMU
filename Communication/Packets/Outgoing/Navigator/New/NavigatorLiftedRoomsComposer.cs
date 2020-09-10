namespace Plus.Communication.Packets.Outgoing.Navigator.New
{
    class NavigatorLiftedRoomsComposer : MessageComposer
    {
        public NavigatorLiftedRoomsComposer()
            : base(ServerPacketHeader.NavigatorLiftedRoomsMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0); //Count
            {
                packet.WriteInteger(1); //Flat Id
                packet.WriteInteger(0); //Unknown
                packet.WriteString(string.Empty); //Image
                packet.WriteString("Caption"); //Caption.
            }
        }
    }
}
