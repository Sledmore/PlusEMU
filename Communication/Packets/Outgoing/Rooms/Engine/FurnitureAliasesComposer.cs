namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class FurnitureAliasesComposer : ServerPacket
    {
        public FurnitureAliasesComposer()
            : base(ServerPacketHeader.FurnitureAliasesMessageComposer)
        {
            base.WriteInteger(0);          
        }
    }
}
