using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class InstantMessageErrorComposer : ServerPacket
    {
        public InstantMessageErrorComposer(MessengerMessageErrors Error, int Target)
            : base(ServerPacketHeader.InstantMessageErrorMessageComposer)
        {
            WriteInteger(MessengerMessageErrorsUtility.GetMessageErrorPacketNum(Error));
            WriteInteger(Target);
           WriteString("");
        }
    }
}
