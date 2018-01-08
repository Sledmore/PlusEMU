using Plus.Communication.Packets.Outgoing.GameCenter;

namespace Plus.Communication.Packets.Incoming.GameCenter
{
    class GetPlayableGamesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GameId = Packet.PopInt();

            Session.SendPacket(new GameAccountStatusComposer(GameId));
            Session.SendPacket(new PlayableGamesComposer(GameId));
            Session.SendPacket(new GameAchievementListComposer(Session, PlusEnvironment.GetGame().GetAchievementManager().GetGameAchievements(GameId), GameId));
        }
    }
}