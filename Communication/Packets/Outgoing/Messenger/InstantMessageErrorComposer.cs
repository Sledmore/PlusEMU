using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class InstantMessageErrorComposer : ServerPacket
    {
        public InstantMessageErrorComposer(MessengerMessageErrors Error, int Target)
            : base(ServerPacketHeader.InstantMessageErrorMessageComposer)
        {
            base.WriteInteger(MessengerMessageErrorsUtility.GetMessageErrorPacketNum(Error));
            base.WriteInteger(Target);
           base.WriteString("");
        }
    }
}
