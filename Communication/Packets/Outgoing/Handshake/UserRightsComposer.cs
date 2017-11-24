namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class UserRightsComposer : ServerPacket
    {
        public UserRightsComposer(int Rank)
            : base(ServerPacketHeader.UserRightsMessageComposer)
        {
            base.WriteInteger(2);//Club level
            base.WriteInteger(Rank);
            base.WriteBoolean(false);//Is an ambassador
        }
    }
}