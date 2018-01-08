namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class CheckGnomeNameComposer : ServerPacket
    {
        public CheckGnomeNameComposer(string PetName, int ErrorId)
            : base(ServerPacketHeader.CheckGnomeNameMessageComposer)
        {
            WriteInteger(0);
            WriteInteger(ErrorId);
           WriteString(PetName);
        }
    }
}
