namespace Plus.Communication.Packets.Outgoing.Navigator.New
{
    class NavigatorPreferencesComposer : ServerPacket
    {
        public NavigatorPreferencesComposer()
            : base(ServerPacketHeader.NavigatorPreferencesMessageComposer)
        {
            base.WriteInteger(68);//X
            base.WriteInteger(42);//Y
            base.WriteInteger(425);//Width
            base.WriteInteger(592);//Height
            base.WriteBoolean(false);//Show or hide saved searches.
            base.WriteInteger(0);//No idea?
        }
    }
}

