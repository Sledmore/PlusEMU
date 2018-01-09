namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces
{
    public class RentableSpaceComposer : ServerPacket
    {
        public RentableSpaceComposer()
            : base(ServerPacketHeader.RentableSpaceMessageComposer)
        {
            WriteBoolean(true); //Is rented y/n
            WriteInteger(-1); //No fucking clue
            WriteInteger(-1); //No fucking clue
           WriteString("Tyler-Retros"); //Username of who owns.
            WriteInteger(360); //Time to expire.
            WriteInteger(-1); //No fucking clue
        }
    }
}