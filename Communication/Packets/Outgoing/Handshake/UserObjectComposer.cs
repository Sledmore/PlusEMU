using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class UserObjectComposer : MessageComposer
    {
        public Habbo Habbo { get; }

        public UserObjectComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserObjectMessageComposer)
        {
            this.Habbo = Habbo;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Habbo.Id);
            packet.WriteString(Habbo.Username);
            packet.WriteString(Habbo.Look);
            packet.WriteString(Habbo.Gender.ToUpper());
            packet.WriteString(Habbo.Motto);
            packet.WriteString("");
            packet.WriteBoolean(false);
            packet.WriteInteger(Habbo.GetStats().Respect);
            packet.WriteInteger(Habbo.GetStats().DailyRespectPoints);
            packet.WriteInteger(Habbo.GetStats().DailyPetRespectPoints);
            packet.WriteBoolean(false); // Friends stream active
            packet.WriteString(Habbo.LastOnline.ToString()); // last online?
            packet.WriteBoolean(Habbo.ChangingName); // Can change name
            packet.WriteBoolean(false);
        }
    }
}