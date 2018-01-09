using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class UserObjectComposer : ServerPacket
    {
        public UserObjectComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserObjectMessageComposer)
        {
            WriteInteger(Habbo.Id);
            WriteString(Habbo.Username);
            WriteString(Habbo.Look);
            WriteString(Habbo.Gender.ToUpper());
            WriteString(Habbo.Motto);
            WriteString("");
            WriteBoolean(false);
            WriteInteger(Habbo.GetStats().Respect);
            WriteInteger(Habbo.GetStats().DailyRespectPoints);
            WriteInteger(Habbo.GetStats().DailyPetRespectPoints);
            WriteBoolean(false); // Friends stream active
            WriteString(Habbo.LastOnline.ToString()); // last online?
            WriteBoolean(Habbo.ChangingName); // Can change name
            WriteBoolean(false);
        }
    }
}