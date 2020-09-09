using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class ManageGroupComposer : MessageComposer
    {
        public Group Group { get; }
        public string[] BadgeParts { get; }

        public ManageGroupComposer(Group Group, string[] BadgeParts)
            : base(ServerPacketHeader.ManageGroupMessageComposer)
        {
            this.Group = Group;
            this.BadgeParts = BadgeParts;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
            packet.WriteBoolean(true);
            packet.WriteInteger(Group.Id);
            packet.WriteString(Group.Name);
            packet.WriteString(Group.Description);
            packet.WriteInteger(1);
            packet.WriteInteger(Group.Colour1);
            packet.WriteInteger(Group.Colour2);
            packet.WriteInteger(Group.Type == GroupType.Open ? 0 : Group.Type == GroupType.Locked ? 1 : 2);
            packet.WriteInteger(Group.AdminOnlyDeco);
            packet.WriteBoolean(false);
            packet.WriteString("");

            packet.WriteInteger(5);

            for (int x = 0; x < BadgeParts.Length; x++)
            {
                string symbol = BadgeParts[x];

                packet.WriteInteger((symbol.Length >= 6) ? int.Parse(symbol.Substring(0, 3)) : int.Parse(symbol.Substring(0, 2)));
                packet.WriteInteger((symbol.Length >= 6) ? int.Parse(symbol.Substring(3, 2)) : int.Parse(symbol.Substring(2, 2)));
                packet.WriteInteger(symbol.Length < 5 ? 0 : symbol.Length >= 6 ? int.Parse(symbol.Substring(5, 1)) : int.Parse(symbol.Substring(4, 1)));
            }

            int i = 0;
            while (i < (5 - BadgeParts.Length))
            {
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                i++;
            }

            packet.WriteString(Group.Badge);
            packet.WriteInteger(Group.MemberCount);
        }
    }
}