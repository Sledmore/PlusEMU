namespace Plus.Communication.Packets.Outgoing.Navigator.New
{
    class NavigatorPreferencesComposer : ServerPacket
    {
        public NavigatorPreferencesComposer()
            : base(ServerPacketHeader.NavigatorPreferencesMessageComposer)
        {
            // TODO: To Sleddy: Shouldn't we make this savable at some point?
            // TODO: HMU if you want it to be saved to the database
            WriteInteger(68);//X
            WriteInteger(42);//Y
            WriteInteger(425);//Width
            WriteInteger(592);//Height
            WriteBoolean(false);//Show or hide saved searches.
            WriteInteger(0);//No idea?
        }
    }
}

