using System.Collections.Generic;

using Plus.HabboHotel.Games;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class GameListComposer : MessageComposer
    {
        public ICollection<GameData> Games { get; }

        public GameListComposer(ICollection<GameData> Games)
            : base(ServerPacketHeader.GameListMessageComposer)
        {
            this.Games = Games;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(PlusEnvironment.GetGame().GetGameDataManager().GetCount());//Game count
            foreach (GameData Game in Games)
            {
                packet.WriteInteger(Game.Id);
                packet.WriteString(Game.Name);
                packet.WriteString(Game.ColourOne);
                packet.WriteString(Game.ColourTwo);
                packet.WriteString(Game.ResourcePath);
                packet.WriteString(Game.StringThree);
            }
        }
    }
}
