using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class InstantMessageErrorComposer : MessageComposer
    {
        public MessengerMessageErrors Error { get; }
        public int Target { get; }

        public InstantMessageErrorComposer(MessengerMessageErrors Error, int Target)
            : base(ServerPacketHeader.InstantMessageErrorMessageComposer)
        {
            this.Error = Error;
            this.Target = Target;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(MessengerMessageErrorsUtility.GetMessageErrorPacketNum(Error));
            packet.WriteInteger(Target);
            packet.WriteString("");
        }
    }
}
