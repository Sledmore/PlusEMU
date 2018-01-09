using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.AI.Speech;


namespace Plus.Communication.Packets.Incoming.Rooms.AI.Bots
{
    class OpenBotActionEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            int botId = packet.PopInt();
            int actionId = packet.PopInt();

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            if (!room.GetRoomUserManager().TryGetBot(botId, out RoomUser botUser))
                return;

            string botSpeech = "";
            foreach (RandomSpeech speech in botUser.BotData.RandomSpeech.ToList())
            {
                botSpeech += (speech.Message + "\n");
            }

            botSpeech += ";#;";
            botSpeech += botUser.BotData.AutomaticChat;
            botSpeech += ";#;";
            botSpeech += botUser.BotData.SpeakingInterval;
            botSpeech += ";#;";
            botSpeech += botUser.BotData.MixSentences;

            if (actionId == 2 || actionId == 5)
                session.SendPacket(new OpenBotActionComposer(botUser, actionId, botSpeech));
        }
    }
}