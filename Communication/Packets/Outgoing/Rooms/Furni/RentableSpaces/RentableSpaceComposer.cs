namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces
{
    public class RentableSpaceComposer : MessageComposer
    {
        public RentableSpaceComposer()
            : base(ServerPacketHeader.RentableSpaceMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(true); //Is rented y/n
            packet.WriteInteger(-1); //No fucking clue
            packet.WriteInteger(-1); //No fucking clue
            packet.WriteString("Tyler-Retros"); //Username of who owns.
            packet.WriteInteger(360); //Time to expire.
            packet.WriteInteger(-1); //No fucking clue
        }
    }
}