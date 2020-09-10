namespace Plus.Communication.Packets.Outgoing.Navigator.New
{
    class NavigatorPreferencesComposer : MessageComposer
    {
        public NavigatorPreferencesComposer()
            : base(ServerPacketHeader.NavigatorPreferencesMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            // TODO: To Sleddy: Shouldn't we make this savable at some point?
            // TODO: HMU if you want it to be saved to the database
            packet.WriteInteger(68);//X
            packet.WriteInteger(42);//Y
            packet.WriteInteger(425);//Width
            packet.WriteInteger(592);//Height
            packet.WriteBoolean(false);//Show or hide saved searches.
            packet.WriteInteger(0);//No idea?
        }
    }
}

