using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class NewBuddyRequestComposer : ServerPacket
    {
        public NewBuddyRequestComposer(UserCache Habbo)
            : base(ServerPacketHeader.NewBuddyRequestMessageComposer)
        {
            WriteInteger(Habbo.Id);
           WriteString(Habbo.Username);
           WriteString(Habbo.Look);
        }
    }
}
