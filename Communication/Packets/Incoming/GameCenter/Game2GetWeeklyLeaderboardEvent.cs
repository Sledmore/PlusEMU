using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Games;
using Plus.Communication.Packets.Outgoing.GameCenter;
using System.Data;

using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Incoming.GameCenter
{
    class Game2GetWeeklyLeaderboardEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GameId = Packet.PopInt();

            GameData GameData = null;
            if (PlusEnvironment.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData))
            {
                //Code
            }
        }
    }
}
