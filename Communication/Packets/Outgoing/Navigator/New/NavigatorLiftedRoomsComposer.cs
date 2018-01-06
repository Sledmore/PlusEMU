namespace Plus.Communication.Packets.Outgoing.Navigator.New
{
    class NavigatorLiftedRoomsComposer : ServerPacket
    {
        public NavigatorLiftedRoomsComposer()
            : base(ServerPacketHeader.NavigatorLiftedRoomsMessageComposer)
        {
            WriteInteger(0); //Count
            {
                WriteInteger(1); //Flat Id
                WriteInteger(0); //Unknown
                WriteString(string.Empty); //Image
                WriteString("Caption"); //Caption.
            }
        }
    }
}
