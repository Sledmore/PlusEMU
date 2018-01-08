using System.Collections.Generic;

using Plus.HabboHotel.Games;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class GameListComposer : ServerPacket
    {
        public GameListComposer(ICollection<GameData> Games)
            : base(ServerPacketHeader.GameListMessageComposer)
        {
            WriteInteger(PlusEnvironment.GetGame().GetGameDataManager().GetCount());//Game count
            foreach (GameData Game in Games)
            {
                WriteInteger(Game.Id);
               WriteString(Game.Name);
               WriteString(Game.ColourOne);
               WriteString(Game.ColourTwo);
               WriteString(Game.ResourcePath);
               WriteString(Game.StringThree);
            }
        }
    }
}
