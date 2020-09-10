namespace Plus.Communication.Packets.Outgoing.Navigator.New
{
    class NavigatorCollapsedCategoriesComposer : MessageComposer
    {
        public NavigatorCollapsedCategoriesComposer()
            : base(ServerPacketHeader.NavigatorCollapsedCategoriesMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
        }
    }
}
