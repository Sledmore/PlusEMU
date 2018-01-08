namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces
{
    public class RentableSpaceComposer : ServerPacket
    {
        public RentableSpaceComposer()
            : base(ServerPacketHeader.RentableSpaceMessageComposer)
        {
            base.WriteBoolean(true); //Is rented y/n
            base.WriteInteger(-1); //No fucking clue
            base.WriteInteger(-1); //No fucking clue
           base.WriteString("Tyler-Retros"); //Username of who owns.
            base.WriteInteger(360); //Time to expire.
            base.WriteInteger(-1); //No fucking clue
        }
    }
}