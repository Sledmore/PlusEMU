using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Bots
{
    class OpenBotActionComposer : ServerPacket
    {
        public OpenBotActionComposer(RoomUser BotUser, int ActionId, string BotSpeech)
            : base(ServerPacketHeader.OpenBotActionMessageComposer)
        {
            WriteInteger(BotUser.BotData.Id);
            WriteInteger(ActionId);
            if (ActionId == 2)
               WriteString(BotSpeech);
            else if (ActionId == 5)
               WriteString(BotUser.BotData.Name);
        }
    }
}
