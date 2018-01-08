namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CheckPetNameComposer : ServerPacket
    {
        public CheckPetNameComposer(int Error, string ExtraData)
            : base(ServerPacketHeader.CheckPetNameMessageComposer)
        {
            WriteInteger(Error);//0 = nothing, 1 = too long, 2 = too short, 3 = invalid characters
           WriteString(ExtraData);
        }
    }
}